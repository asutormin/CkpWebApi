using Microsoft.EntityFrameworkCore;
using CkpDAL.Entities;
using CkpDAL.Entities.String;

namespace CkpDAL
{
    public class BPFinanceContext : DbContext
    {
        public BPFinanceContext(DbContextOptions<BPFinanceContext> options)
            : base(options)
        {
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<RubricVersionable> RubricsVersionable { get; set; }
        public DbSet<RubricActual> RubricsActual { get; set; }
        public DbSet<PricePositionType> PricePositionTypes { get; set; }
        public DbSet<PricePositionVersionable> PricePositionsVersionable { get; set; }
        public DbSet<PricePositionActual> PricePositionsActual { get; set; }
        public DbSet<PackagePosition> PackagePositions { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Graphic> Graphics { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountPosition> AccountPositions { get; set; }
        public DbSet<AccountOrder> AccountOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPosition> OrderPositions { get; set; }
        public DbSet<GraphicPosition> GraphicPositions { get; set; }
        public DbSet<RubricPosition> RubricPositions { get; set; }
        public DbSet<BusinessUnit> BusinessUnits { get; set; }
        public DbSet<BusinessUnitCompanyManager> BusinessUnitCompanyManagers { get; set; }

        public DbSet<LegalPerson> LegalPersons { get; set; }
        public DbSet<LoginSettings> LoginSettings { get; set; }
        public DbSet<UnloadingPosition> UnloadingPositions { get; set; }

        public DbSet<SupplierProject> SupplierProjects { get; set; }
        public DbSet<SupplierProjectRelation> SupplierProjectRelations { get; set; }
        public DbSet<SupplierProjectRubric> SupplierProjectRubrics { get; set; }

        public DbSet<OrderIm> OrderIms { get; set; }
        public DbSet<OrderImType> OrderImTypes { get; set; }
        public DbSet<PositionIm> PositionIms { get; set; }
        public DbSet<PositionImType> PositionImTypes { get; set; }

        public DbSet<StringPosition> StringPositions { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<StringPhone> StringPhones { get; set; }
        public DbSet<Web> Webs { get; set; }
        public DbSet<StringWeb> StringWebs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<StringAddress> StringAddresses { get; set; }
        public DbSet<StringOccurrence> StringOccurrences { get; set; }

        public DbSet<Handbook> Handbooks { get; set; }
        public DbSet<HandbookRelation> HandbookRelations { get; set; }
        public DbSet<Metro> Metros { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RubricVersionable>()
                .HasKey(r => new { r.Id, r.BeginDate });

            modelBuilder.Entity<PricePositionVersionable>()
                .HasKey(pp => new { pp.Id, pp.BeginDate });

            modelBuilder.Entity<OrderIm>()
                .HasKey(oi => new { oi.OrderId, oi.OrderImTypeId });

            modelBuilder.Entity<PositionIm>()
                .HasKey(pi => new { pi.OrderPositionId, pi.PositionImTypeId });

            modelBuilder.Entity<StringAddress>()
                .HasKey(sa => new { sa.StringId, sa.AddressId });

            modelBuilder.Entity<StringOccurrence>()
                .HasKey(so => new { so.StringId, so.OccurrenceId });

            modelBuilder.Entity<StringPhone>()
                .HasKey(sp => new { sp.StringId, sp.PhoneId });

            modelBuilder.Entity<StringWeb>()
                .HasKey(sw => new { sw.StringId, sw.WebId });

            modelBuilder.Entity<HandbookRelation>()
                .HasKey(hr => new { hr.CompanyId, hr.HandbookId, hr.HandbookTypeId });

            modelBuilder.Entity<PricePositionActual>()
                .HasOne(pp => pp.Supplier)
                .WithMany(su => su.PricePositions);

            modelBuilder.Entity<PackagePosition>()
                .HasOne(pkg => pkg.PricePosition)
                .WithMany(pp => pp.PackagePositions)
                .HasForeignKey(pkg => pkg.PricePositionId);

            modelBuilder.Entity<PackagePosition>()
                .HasOne(pkg => pkg.Price)
                .WithMany(p => p.PackagePositions)
                .HasForeignKey(pkg => pkg.PriceId);

            modelBuilder.Entity<Price>()
                .HasOne(p => p.PricePosition)
                .WithMany(pp => pp.Prices);

            modelBuilder.Entity<Graphic>()
                .HasOne(g => g.PricePositionType)
                .WithMany(ppt => ppt.Graphics);

            modelBuilder.Entity<Graphic>()
                .HasOne(g => g.Supplier);

            modelBuilder.Entity<GraphicPosition>()
                .HasMany(gp => gp.ChildGraphicPositions)
                .WithOne(cgp => cgp.ParentGraphicPosition);

            modelBuilder.Entity<Account>()
                .HasMany(ac => ac.AccountPositions)
                .WithOne(ap => ap.Account);

            modelBuilder.Entity<Account>()
                .HasMany(ac => ac.AccountOrders)
                .WithOne(ao => ao.Account);

            modelBuilder.Entity<AccountOrder>()
                .HasOne(ao => ao.Order)
                .WithOne(o => o.AccountOrder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.AccountOrder)
                .WithOne(ao => ao.Order);

            modelBuilder.Entity<OrderPosition>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderPositions);

            modelBuilder.Entity<OrderPosition>()
                .HasMany(op => op.ChildOrderPositions)
                .WithOne(cop => cop.ParentOrderPosition);

            modelBuilder.Entity<GraphicPosition>()
                .HasOne(gp => gp.OrderPosition)
                .WithMany(op => op.GraphicPositions);

            modelBuilder.Entity<RubricPosition>()
                .HasOne(rp => rp.OrderPosition)
                .WithMany(op => op.RubricPositions);

            modelBuilder.Entity<AccountSettings>()
                .HasOne(acs => acs.Account)
                .WithOne(ac => ac.AccountSettings);

            modelBuilder.Entity<AccountSettings>()
                .HasOne(acs => acs.LegalPersonBank);

            modelBuilder.Entity<LegalPerson>()
                .HasOne(lp => lp.AccountSettings)
                .WithOne(acs => acs.LegalPerson);

            modelBuilder.Entity<LegalPerson>()
                .HasMany(lp => lp.LegalPersonBanks)
                .WithOne(lpb => lpb.LegalPerson);

            modelBuilder.Entity<UnloadingPosition>()
                .HasOne(up => up.GraphicPosition)
                .WithOne(gp => gp.UnloadingPosition);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderIms)
                .WithOne(oi => oi.Order);

            modelBuilder.Entity<OrderImType>()
                .HasMany(oit => oit.PositionImTypes)
                .WithOne(pit => pit.OrderImType);

            modelBuilder.Entity<PositionImType>()
                .HasMany(pit => pit.PricePositionTypes)
                .WithOne(ppt => ppt.PositionImType);

            modelBuilder.Entity<PositionIm>()
                .HasOne(pi => pi.StringPosition)
                .WithOne(sp => sp.PositionIm)
                .HasPrincipalKey<PositionIm>(pi => pi.OrderPositionId)
                .HasForeignKey<StringPosition>(sp => sp.OrderPositionId);

            modelBuilder.Entity<StringPosition>()
                .HasOne(sp => sp.PositionIm)
                .WithOne(pi => pi.StringPosition)
                .HasPrincipalKey<StringPosition>(sp => sp.OrderPositionId)
                .HasForeignKey<PositionIm>(pi => pi.OrderPositionId);

            modelBuilder.Entity<StringPosition>()
                .HasMany(s => s.Addresses)
                .WithOne(sa => sa.StringPosition);

            modelBuilder.Entity<StringPosition>()
                .HasMany(s => s.Occurrences)
                .WithOne(so => so.StringPosition);

            modelBuilder.Entity<StringPosition>()
                .HasMany(s => s.Phones)
                .WithOne(sp => sp.StringPosition);

            modelBuilder.Entity<StringPosition>()
                .HasMany(s => s.Webs)
                .WithOne(sw => sw.StringPosition);

            modelBuilder.Entity<SupplierProjectRelation>()
                .HasOne(spr => spr.Price)
                .WithOne(p => p.SupplierProjectRelation);
        }
    }
}
