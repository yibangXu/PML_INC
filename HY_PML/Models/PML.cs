namespace HY_PML.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PML : DbContext
    {
        public PML()
            : base("name=PML")
        {
        }

		public virtual DbSet<Bill_Lading> Bill_Lading { get; set; }
		public virtual DbSet<Bill_Land_Mas> Bill_Land_Mas { get; set; }
        public virtual DbSet<Bill_Land_Dtl> Bill_Land_Dtl { get; set; }
		public virtual DbSet<DeclCust_Main> DeclCust_Main { get; set; }
		public virtual DbSet<DeclCust_Sub> DeclCust_Sub { get; set; }
		public virtual DbSet<Export_Bill> Export_Bill { get; set; }
		public virtual DbSet<Import_Bill> Import_Bill { get; set; }
		public virtual DbSet<ORG_Area> ORG_Area { get; set; }
        public virtual DbSet<ORG_Banks> ORG_Banks { get; set; }
        public virtual DbSet<ORG_Cc> ORG_Cc { get; set; }
        public virtual DbSet<ORG_Company> ORG_Company { get; set; }
        public virtual DbSet<ORG_Consin> ORG_Consin { get; set; }
        public virtual DbSet<ORG_Currency> ORG_Currency { get; set; }
        public virtual DbSet<ORG_Cust> ORG_Cust { get; set; }
        public virtual DbSet<ORG_CustDetail> ORG_CustDetail { get; set; }
        public virtual DbSet<ORG_Depart> ORG_Depart { get; set; }
        public virtual DbSet<ORG_Dest> ORG_Dest { get; set; }
        public virtual DbSet<ORG_Feelist> ORG_Feelist { get; set; }
        public virtual DbSet<ORG_Hub> ORG_Hub { get; set; }
        public virtual DbSet<ORG_PickUpArea> ORG_PickUpArea { get; set; }
        public virtual DbSet<ORG_PickUpAreaAddress> ORG_PickUpAreaAddress { get; set; }
        public virtual DbSet<ORG_PostCode> ORG_PostCode { get; set; }
        public virtual DbSet<ORG_Product> ORG_Product { get; set; }
        public virtual DbSet<ORG_Sector> ORG_Sector { get; set; }
        public virtual DbSet<ORG_Sector_OLD> ORG_Sector_OLD { get; set; }
        public virtual DbSet<ORG_SectorAbsent> ORG_SectorAbsent { get; set; }
        public virtual DbSet<ORG_Stat> ORG_Stat { get; set; }
        public virtual DbSet<ORG_Vehicle> ORG_Vehicle { get; set; }
        public virtual DbSet<Shdet> Shdet { get; set; }
        public virtual DbSet<ShdetDetail> ShdetDetail { get; set; }
        public virtual DbSet<ShdetHeader> ShdetHeader { get; set; }
        public virtual DbSet<ShdetProd> ShdetProd { get; set; }
        public virtual DbSet<SYS_ActInfo> SYS_ActInfo { get; set; }
        public virtual DbSet<SYS_ActUser> SYS_ActUser { get; set; }
        public virtual DbSet<SYS_ActUserGroup> SYS_ActUserGroup { get; set; }
        public virtual DbSet<SYS_Menu> SYS_Menu { get; set; }
        public virtual DbSet<SYS_MenuItem> SYS_MenuItem { get; set; }
        public virtual DbSet<SYS_User> SYS_User { get; set; }
        public virtual DbSet<SYS_UserDataGridField> SYS_UserDataGridField { get; set; }
        public virtual DbSet<SYS_UserGroup> SYS_UserGroup { get; set; }
		public virtual DbSet<Transportation> Transportation { get; set; }
		public virtual DbSet<TransportationD> TransportationD { get; set; }
		public virtual DbSet<TransportationR> TransportationR { get; set; }
		public virtual DbSet<TransportationA> TransportationA { get; set; }
		public virtual DbSet<TransportationS> TransportationS { get; set; }
		public virtual DbSet<Cash_Receive> Cash_Receive { get; set; }
		public virtual DbSet<Bill_Receive> Bill_Receive { get; set; }
		public virtual DbSet<Discount> Discount { get; set; }
		public virtual DbSet<ORG_Report_Mgmt> ORG_Report_Mgmt { get; set; }
		public virtual DbSet<BL_Report_Mas> BL_Report_Mas { get; set; }
		public virtual DbSet<BL_Report_Dtl> BL_Report_Dtl { get; set; }
		public virtual DbSet<BL_FileTable> BL_FileTable { get; set; }
		public virtual DbSet<Bill_Lading_Record> Bill_Lading_Record { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SYS_Menu>()
                .HasMany(e => e.SYS_MenuItem)
                .WithRequired(e => e.SYS_Menu)
                .HasForeignKey(e => e.MenuID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SYS_MenuItem>()
                .HasMany(e => e.SYS_MenuItem1)
                .WithOptional(e => e.SYS_MenuItem2)
                .HasForeignKey(e => e.ParentID);
        }
    }
}
