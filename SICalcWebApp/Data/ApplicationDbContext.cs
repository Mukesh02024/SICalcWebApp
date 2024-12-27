using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Models;


namespace SICalcWebApp.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            this.Database.SetCommandTimeout(120);

        }
        public DbSet<FC> FCs { get; set; }
        public DbSet<TPDInfo> TPDInfos { get; set; }
        public DbSet<FCInfo> FCInfos { get; set; }

        public DbSet<IronType> IronTypes { get; set; }

        public DbSet<InputOperand> InputOperands { get; set; }

        public DbSet<PriceOfMaterial> PriceOfMaterials { get; set; }

        public DbSet<GroupMill> GroupMills { get; set; }
        public DbSet<MillItem> MillItems { get; set; }

        public DbSet<HmaliInput> HmaliInputs { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<PaddyType> PaddyTypes { get; set; }

        public DbSet<Staff> Staffs{ get; set; }



        public DbSet<MillBunker> MillBunkers { get; set; }
        public DbSet<SortexBunker> SortexBunkers { get; set; }
        public DbSet<TypeOfHandi> TypeOfHandis { get; set; }
        public DbSet<ProcessMethod> ProcessMethods{ get; set; }
        public DbSet<FeedingBunker> FeedingBunkers { get; set; }


        public DbSet<FeedingModuleF> FeedingModuleFs { get; set; }



        public DbSet<HandiProcess> HandiProcesses{ get; set; }
        public DbSet<DryerProcess> DryerProcesses { get; set; }

        public DbSet<MillingProcess> MillingProcesses { get; set; }

        public DbSet<SortexProcess> SortexProcesses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupMill>()
               .HasKey(g => g.GroupId);

            modelBuilder.Entity<MillItem>()
                .HasKey(i => i.ItemId);

            modelBuilder.Entity<HmaliInput>()
                .HasKey(h => h.HmaliId); // Assuming you add an Id property

            // Define relationships
            modelBuilder.Entity<MillItem>()
                .HasOne(i => i.GroupMill)
                .WithMany(g => g.MillItems)
                .HasForeignKey(i => i.GroupId);



            modelBuilder.Entity<InputOperand>()
                .HasOne(io => io.IronType)
                .WithMany()
                .HasForeignKey(io => io.IronTypeId);
            // Configure the FC entity
            modelBuilder.Entity<FC>()
                .HasKey(fc => fc.FCId);

            modelBuilder.Entity<FC>()
                .Property(fc => fc.C_Fe)
                .HasPrecision(18, 2);

            // Configure the TPDInfo entity
            modelBuilder.Entity<TPDInfo>()
                .HasKey(tpd => tpd.TPDId);

            // Configure the FCInfo entity
            modelBuilder.Entity<FCInfo>()
                .HasKey(fcInfo => fcInfo.FCInfoId);

            modelBuilder.Entity<FCInfo>()
                .HasOne(fcInfo => fcInfo.FC)
                .WithMany(fc => fc.FCInfos)
                .HasForeignKey(fcInfo => fcInfo.FCId);

            modelBuilder.Entity<FCInfo>()
                .HasOne(fcInfo => fcInfo.TPDInfo)
                .WithMany(tpd => tpd.FCInfos)
                .HasForeignKey(fcInfo => fcInfo.TPDId);

            modelBuilder.Entity<FCInfo>()
                .Property(fcInfo => fcInfo.FeedRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity <InputOperand>().Property(inputOperand=>inputOperand.FeMSponge).HasPrecision(18, 4);
            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.FeT).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.FineLoss).HasPrecision(18, 4);

        

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.FinesRealisationValue).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.Gangue).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.GroundLoss).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.IronPrice).HasPrecision(18, 2);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.Moisture).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.Phos).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.TransferLoss).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.Yield).HasPrecision(18, 4);

            modelBuilder.Entity<InputOperand>().Property(inputOperand => inputOperand.YLoss).HasPrecision(18, 4);


        }
  

    }
}
