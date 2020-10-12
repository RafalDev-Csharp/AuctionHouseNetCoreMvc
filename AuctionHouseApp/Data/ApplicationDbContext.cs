using System;
using System.Collections.Generic;
using System.Text;
using AuctionHouseApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set;  }
        public DbSet<ItemQuality> ItemQualityDb { get; set;  }
        public DbSet<ForWhichClassItem> ForWhichClassItemDb { get; set;  }
        public DbSet<GameItem> GameItemsDb { get; set; }
        public DbSet<SellItem> SellItemDb { get; set; }
        public DbSet<Inventory> InventoryDb { get; set; }
        public DbSet<GameUser> GameUserDb { get; set; }
        public DbSet<CurrencyBundle> CurrencyBundleDb { get; set; }

    }
}
