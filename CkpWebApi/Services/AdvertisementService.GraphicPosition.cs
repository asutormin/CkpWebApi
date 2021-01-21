using CkpWebApi.DAL.Model;
using CkpWebApi.Helpers;
using CkpWebApi.InputEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CkpWebApi.Services
{
    public partial class AdvertisementService
    {
        #region Create

        private bool NeedCreateGraphicPosition(IEnumerable<GraphicPosition> graphicPositions, int advGraphicId)
        {
            return !graphicPositions.Any(gp => gp.GraphicId == advGraphicId);
        }

        private int CreateGraphicPosition(int orderPositionId, int parentGraphicPositionId, int graphicId, DbTransaction dbTran)
        {
            var graphicPositionId = 0;
            var lastEditDate = DateTime.Now;

            _repository.SetGraphicPosition(
                dbTran: dbTran,
                ref graphicPositionId,
                parentGraphicPositionId: parentGraphicPositionId,
                orderPositionId: orderPositionId,
                graphicId: graphicId,
                countPosition: 1,
                editUserId: _editUserId,
                isActual: true,
                ref lastEditDate);

            SetNeedRefreshOrderShoppingCartOrder();

            return graphicPositionId;
        }

        private void CreateGraphicPositions(int orderPositionId, List<AdvertisementGraphic> advGraphics, DbTransaction dbTran)
        {
            // Перебираем новые графики
            foreach (var advGraphic in advGraphics)
            {
                // Добавляем новую позицию графика
                var graphicPositionId = CreateGraphicPosition(orderPositionId, 0, advGraphic.Id, dbTran);

                // Перебираем дочерние графики переданного графика
                foreach (var childGraphicId in advGraphic.Childs)
                    // Добавляем новую дочернюю позицию графика
                    CreateGraphicPosition(orderPositionId, graphicPositionId, childGraphicId, dbTran);

            }
        }

        #endregion

        #region Update
        private void UpdateGraphicPositions(int orderPositionId, IEnumerable<GraphicPosition> graphicPositions, List<AdvertisementGraphic> advGraphics, DbTransaction dbTran)
        {
            var parentGraphicPositions = graphicPositions
                .Where(gp => gp.ParenGraphicPositiontId == gp.Id);

            var parentGraphicPositionList = parentGraphicPositions.ToList();

            // Перебираем существующие позиции графиков
            for (int i = parentGraphicPositionList.Count - 1; i >=0; i--)
            {
                var parentGraphicPosition = parentGraphicPositionList[i];

                // Если среди переданных графиков нет графика позиции графиков
                var advGraphicIds = advGraphics.Select(gr => gr.Id).ToList();
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
            foreach (var advGraphic in advGraphics)
            {
                // Ищем существующую позицию графика с переданным графиком
                var graphicPosition = _context.GraphicPositions
                    .Include(gp => gp.ChildGraphicPositions)
                    .Where(
                        gp =>
                            gp.OrderPositionId == orderPositionId &&
                            gp.ParenGraphicPositiontId == gp.Id &&
                            gp.GraphicId == advGraphic.Id)
                    .SingleOrDefault();

                if (NeedCreateGraphicPosition(parentGraphicPositions, advGraphic.Id))
                {
                    // Если позиция графика с переданным графиком не существует

                    // Добавляем новую позицию графика
                    var graphicPositionId = CreateGraphicPosition(orderPositionId, 0, advGraphic.Id, dbTran);

                    // Перебираем дочерние графики переданного графика
                    foreach (var childGraphicId in advGraphic.Childs)
                        // Добавляем новую дочернюю позицию графика
                        CreateGraphicPosition(orderPositionId, graphicPositionId, childGraphicId, dbTran);
                }
                else
                {
                    // Если позиция графика с переданным графиком существует

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
        private void DeleteGraphicPositions(IEnumerable<GraphicPosition> graphicPositions, DbTransaction dbTran)
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
            var graphicPositionId = graphicPosition.Id;
            var lastEditDate = graphicPosition.BeginDate;

            _repository.SetGraphicPosition(
                dbTran: dbTran,
                ref graphicPositionId,
                parentGraphicPositionId: graphicPosition.ParenGraphicPositiontId,
                orderPositionId: graphicPosition.OrderPositionId,
                graphicId: graphicPosition.GraphicId,
                countPosition: graphicPosition.Count,
                editUserId: _editUserId,
                isActual: false,
                ref lastEditDate);

            _context.Entry(graphicPosition).Reload();
            SetNeedRefreshOrderShoppingCartOrder();
        }

        private bool NeedDeleteGraphicPosition(List<int> advGraphicIds, int graphicId)
        {
            return !advGraphicIds.Contains(graphicId);
        }

        #endregion
    }
}
