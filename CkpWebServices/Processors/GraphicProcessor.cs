using CkpDAL;
using CkpDAL.Entities;
using CkpDAL.Repository;
using CkpModel.Input;
using CkpServices.Helpers;
using CkpServices.Helpers.Factories;
using CkpServices.Helpers.Factories.Interfaces;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpServices.Processors
{
    class GraphicProcessor : IGraphicProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        private readonly IGraphicPositionFactory _graphicPositionFactory;

        public GraphicProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;

            _graphicPositionFactory = new GraphicPositionFactory();
        }

        #region Get

        public List<GraphicData> GetPackageGraphicsData(OrderPositionData opd)
        {
            var graphicsData = new List<GraphicData>();
            graphicsData.Add(GetPackageGraphicData(opd));

            return graphicsData;
        }

        private GraphicData GetPackageGraphicData(OrderPositionData opd)
        {
            // Получаем идентификаторы графиков позиций пакета
            var graphicIds = opd.Childs
                .SelectMany(c => c.GraphicsData)
                .Select(g => g.Id);

            // Находим минимальную дату выхода позиций пакета
            // с поставщиком пакета
            var minOutDate = _context.Graphics
                .Where(g => graphicIds.Contains(g.Id) && g.SupplierId == opd.SupplierId)
                .Min(g => g.OutDate);

            // Находим график выхода пакета - его дата выхода меньше
            // или равна миминальной дате выхода позиций пакета
            var graphic = _context.Graphics
                .Where(g =>
                    g.SupplierId == opd.SupplierId &&
                    g.PricePositionTypeId == opd.FormatData.FormatTypeId &&
                    g.OutDate <= minOutDate)
                .OrderByDescending(g => g.OutDate)
                .First();

            var graphicData = new GraphicData { Id = graphic.Id, Childs = new List<int>() };

            return graphicData;
        }

        private GraphicPosition GetGraphicPosition(int orderPositionId, int graphicId)
        {
            return
                _context.GraphicPositions
                        .Include(gp => gp.ChildGraphicPositions)
                        .Where(
                            gp =>
                                gp.OrderPositionId == orderPositionId &&
                                gp.ParenGraphicPositiontId == gp.Id &&
                                gp.GraphicId == graphicId)
                        .SingleOrDefault();
        }

        #endregion

        #region Create

        public void CreateGraphicPositions(int orderPositionId, List<GraphicData> graphicsData, DbTransaction dbTran)
        {
            // Перебираем новые графики
            foreach (var advGraphic in graphicsData)
            {
                // Добавляем новую позицию графика
                var graphicPosition = CreateGraphicPosition(orderPositionId, 0, advGraphic.Id, dbTran);

                // Перебираем дочерние графики переданного графика
                foreach (var childGraphicId in advGraphic.Childs)
                    // Добавляем новую дочернюю позицию графика
                    CreateGraphicPosition(orderPositionId, graphicPosition.Id, childGraphicId, dbTran);
            }
        }

        private GraphicPosition CreateGraphicPosition(int orderPositionId, int parentGraphicPositionId, int graphicId, DbTransaction dbTran)
        {
            var graphicPosition = _graphicPositionFactory.Create(orderPositionId, parentGraphicPositionId, graphicId);
            graphicPosition = _repository.SetGraphicPosition(graphicPosition, isActual: true, dbTran);

            return graphicPosition;
        }

        private bool NeedCreateGraphicPosition(IEnumerable<GraphicPosition> graphicPositions, int graphicId)
        {
            return !graphicPositions.Any(gp => gp.GraphicId == graphicId);
        }

        #endregion

        #region Update

        public void UpdateGraphicPositions(int orderPositionId, IEnumerable<GraphicPosition> graphicPositions, List<GraphicData> graphicsData, DbTransaction dbTran)
        {
            var parentGraphicPositions = graphicPositions
                .Where(gp => gp.ParenGraphicPositiontId == gp.Id);

            var parentGraphicPositionList = parentGraphicPositions.ToList();

            // Перебираем существующие позиции графиков
            for (int i = parentGraphicPositionList.Count - 1; i >= 0; i--)
            {
                var parentGraphicPosition = parentGraphicPositionList[i];

                // Если среди переданных графиков нет графика позиции графиков
                var advGraphicIds = graphicsData.Select(gr => gr.Id).ToList();
                if (NeedDeleteGraphicPosition(advGraphicIds, parentGraphicPosition.GraphicId))
                {
                    var childGraphicPositionsList = parentGraphicPosition.GetChilds().ToList();

                    // Перебираем все дочерние позиции
                    for (var j = childGraphicPositionsList.Count - 1; j >= 0; j--)
                    {
                        var childGraphicPosition = childGraphicPositionsList[j];

                        // Удаляем дочернюю позицию графика
                        DeleteGraphicPosition(childGraphicPosition, dbTran);
                    }

                    // Удаляем позицию графика
                    DeleteGraphicPosition(parentGraphicPosition, dbTran);
                }
            }

            // Перебираем новые графики
            foreach (var advGraphic in graphicsData)
            {
                // Ищем существующую позицию графика с переданным графиком
                var graphicPosition = GetGraphicPosition(orderPositionId, advGraphic.Id);

                // Если позиция графика с переданным графиком не существует
                if (NeedCreateGraphicPosition(parentGraphicPositions, advGraphic.Id))
                {
                    // Добавляем новую позицию графика
                    graphicPosition = CreateGraphicPosition(orderPositionId, 0, advGraphic.Id, dbTran);

                    // Перебираем дочерние графики переданного графика
                    foreach (var childGraphicId in advGraphic.Childs)
                        // Добавляем новую дочернюю позицию графика
                        CreateGraphicPosition(orderPositionId, graphicPosition.Id, childGraphicId, dbTran);
                }
                else // Если позиция графика с переданным графиком существует
                {
                    var childGraphicPositionsList = graphicPosition.GetChilds().ToList();

                    // Перебираем дочерние графики переданного графика
                    foreach (var childAdvGraphicId in advGraphic.Childs)
                        // Если среди дочерних позиций существующей позиции графика
                        // нет дочерней позиции переданного графика
                        if (NeedCreateGraphicPosition(childGraphicPositionsList, childAdvGraphicId))
                            // Добавляем новую дочернюю позицию графика
                            CreateGraphicPosition(orderPositionId, graphicPosition.Id, childAdvGraphicId, dbTran);

                    // Перебираем существующие  дочерние позиции графиков
                    for (int i = childGraphicPositionsList.Count - 1; i >= 0; i--)
                    {
                        var childGraphicPosition = childGraphicPositionsList[i];

                        if (NeedDeleteGraphicPosition(advGraphic.Childs, childGraphicPosition.GraphicId))
                            DeleteGraphicPosition(childGraphicPosition, dbTran);
                    }
                }
            }

            graphicPositions = _context.GraphicPositions
                .Include(gp => gp.Graphic)
                .Where(gp => gp.OrderPositionId == orderPositionId);
        }

        #endregion

        #region Delete

        public void DeleteGraphicPositions(IEnumerable<GraphicPosition> graphicPositions, DbTransaction dbTran)
        {
            var parentGraphicPositions = graphicPositions
                .Where(gp => gp.ParenGraphicPositiontId == gp.Id)
                .ToList();

            for (int i = parentGraphicPositions.Count - 1; i >= 0; i--)
            {
                var childGraphicPositions = parentGraphicPositions[i].GetChilds().ToList();

                for (int j = childGraphicPositions.Count - 1; j >= 0; j--)
                    DeleteGraphicPosition(childGraphicPositions[j], dbTran);

                DeleteGraphicPosition(parentGraphicPositions[i], dbTran);
            }
        }

        private void DeleteGraphicPosition(GraphicPosition graphicPosition, DbTransaction dbTran)
        {
            _repository.SetGraphicPosition(graphicPosition, isActual: false, dbTran);
            _context.Entry(graphicPosition).Reload();
        }

        private bool NeedDeleteGraphicPosition(List<int> advGraphicIds, int graphicId)
        {
            return !advGraphicIds.Contains(graphicId);
        }

        #endregion
    }
}
