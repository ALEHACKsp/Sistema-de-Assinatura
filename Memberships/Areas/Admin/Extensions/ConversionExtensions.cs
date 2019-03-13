using Memberships.Areas.Admin.Models;
using Memberships.Entities;
using Memberships.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Memberships.Areas.Admin.Extensions
{
    public static class ConversionExtensions
    {
        #region Products convert
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products, ApplicationDbContext db)
        {

            if (products.Count().Equals(0))
                return new List<ProductModel>();

            var text = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();

            return from p in products
                   select new ProductModel
                   {
                       Id = p.Id,
                       Title = p.Title,
                       Description = p.Description,
                       ImageUrl = p.ImageUrl,
                       ProductLinkTextId = p.ProductLinkTextId,
                       ProductTypeId = p.ProductTypeId,
                       ProductLinkTexts = text,
                       ProductTypes = types
                   };
        }

        public static async Task<ProductModel> Convert(this Product products, ApplicationDbContext db)
        {


            var text = await db.ProductLinkTexts.FirstOrDefaultAsync(p => p.Id.Equals(products.ProductLinkTextId));
            var types = await db.ProductTypes.FirstOrDefaultAsync(p => p.Id.Equals(products.ProductTypeId));

            var model = new ProductModel
            {
                Id = products.Id,
                Title = products.Title,
                Description = products.Description,
                ImageUrl = products.ImageUrl,
                ProductLinkTextId = products.ProductLinkTextId,
                ProductTypeId = products.ProductTypeId,
                ProductLinkTexts = new List<ProductLinkText>(),
                ProductTypes = new List<ProductType>(),
            };

            model.ProductLinkTexts.Add(text);
            model.ProductTypes.Add(types);

            return model;
        }
        #endregion

        #region ProductItem convert

        public static async Task<IEnumerable<ProductItemModel>> Convert(this IQueryable<ProductItem> productItems, ApplicationDbContext db)
        {

            if (productItems.Count().Equals(0))
                return new List<ProductItemModel>();

            return await (from pi in productItems
                          select new ProductItemModel
                          {
                              ItemId = pi.ItemId,
                              ProductId = pi.ProductId,
                              ItemTitle = db.Items.FirstOrDefault(i => i.Id.Equals(pi.ItemId)).Title,
                              ProductTitle = db.Products.FirstOrDefault(p => p.Id.Equals(pi.ProductId)).Title,

                          }).ToListAsync(); ;

        }

        public static async Task<ProductItemModel> Convert(this ProductItem productItem, ApplicationDbContext db,bool addListData = true)
        {

            var model = new ProductItemModel
            {
                ItemId = productItem.ItemId,
                ProductId = productItem.ProductId,
                Items =  addListData ? await db.Items.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                ItemTitle = (await db.Items.FirstOrDefaultAsync(i=> i.Id.Equals(productItem.ItemId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(p => p.Id.Equals(productItem.ProductId))).Title
            };

            return model;
        }

        public static async Task<bool> CanChange(this ProductItem productItem, ApplicationDbContext db)
        {
            // pega a combinação atual de produto com item do produto
            var oldPI = await db.ProductItems.CountAsync(p => p.ProductId.Equals(productItem.OldProductId) &&
                                                         p.ItemId.Equals(productItem.OldItemId));

            //verifica se a combinação do produto com seu item que estou tenado inserir já  exsiste
            var newPI = await db.ProductItems.CountAsync(p => p.ProductId.Equals(productItem.ProductId) && 
                                                         p.ItemId.Equals(productItem.ItemId));


            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task Change(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldProductitem = await db.ProductItems.FirstOrDefaultAsync(p => p.ProductId.Equals(productItem.OldProductId) &&
                                                                            p.ItemId.Equals(productItem.OldItemId));

            var newProductitem = await db.ProductItems.FirstOrDefaultAsync(p => p.ProductId.Equals(productItem.ProductId) &&
                                                                  p.ItemId.Equals(productItem.ItemId));


            if (oldProductitem != null && newProductitem == null)
            {
                newProductitem = new ProductItem
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldProductitem);
                        db.ProductItems.Add(newProductitem);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch (TransactionException ex )
                    {

                        transaction.Dispose();
                    }
                }
            }

        }

        #endregion

        #region Subscription products convert

        public static async Task<IEnumerable<SubscriptionProductModel>> Convert(this IQueryable<SubscriptionProduct> subscriptionProducts, ApplicationDbContext db)
        {

            if (subscriptionProducts.Count().Equals(0))
                return new List<SubscriptionProductModel>();

            return await (from pi in subscriptionProducts
                          select new SubscriptionProductModel
                          {
                              SubscriptionId = pi.SubscriptionId,
                              ProductId = pi.ProductId,
                              SubscriptionTitle = db.Subscriptions.FirstOrDefault(sp => sp.Id.Equals(pi.SubscriptionId)).Title,
                              ProductTitle = db.Products.FirstOrDefault(p => p.Id.Equals(pi.ProductId)).Title,

                          }).ToListAsync();

        }

        public static async Task<SubscriptionProductModel> Convert(this SubscriptionProduct subscriptionProducts, ApplicationDbContext db, bool addListData = true)
        {

            var model = new SubscriptionProductModel
            {
                SubscriptionId = subscriptionProducts.SubscriptionId,
                ProductId = subscriptionProducts.ProductId,
                Subscriptions = addListData ? await db.Subscriptions.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                SubscriptionTitle = (await db.Subscriptions.FirstOrDefaultAsync(i => i.Id.Equals(subscriptionProducts.SubscriptionId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(p => p.Id.Equals(subscriptionProducts.ProductId))).Title
            };

            return model;
        }

        public static async Task<bool> CanChange(this SubscriptionProduct subscriptionProducts, ApplicationDbContext db)
        {
            // pega a combinação atual de produto com subscription do produto
            var oldSubProd = await db.SubscriptionProducts.CountAsync(sp => sp.ProductId.Equals(subscriptionProducts.OldProductId) &&
                                                         sp.SubscriptionId.Equals(subscriptionProducts.OldSubscriptionId));

            //verifica se a combinação do subscription com seu produto que estou tentando alterar já  exsiste
            var newSubProd = await db.SubscriptionProducts.CountAsync(sp => sp.ProductId.Equals(subscriptionProducts.ProductId) &&
                                                         sp.SubscriptionId.Equals(subscriptionProducts.SubscriptionId));
            

            return oldSubProd.Equals(1) && newSubProd.Equals(0);
        }

        public static async Task Change(this SubscriptionProduct subscriptionProducts, ApplicationDbContext db)
        {
            var oldSubProd = await db.SubscriptionProducts.FirstOrDefaultAsync(sp => sp.ProductId.Equals(subscriptionProducts.OldProductId) &&
                                                                            sp.SubscriptionId.Equals(subscriptionProducts.OldSubscriptionId));

            var newPSubProd = await db.SubscriptionProducts.FirstOrDefaultAsync(sp => sp.ProductId.Equals(subscriptionProducts.ProductId) &&
                                                                  sp.SubscriptionId.Equals(subscriptionProducts.SubscriptionId));


            if (oldSubProd != null && newPSubProd == null)
            {
                newPSubProd = new SubscriptionProduct
                {
                    SubscriptionId = subscriptionProducts.SubscriptionId,
                    ProductId = subscriptionProducts.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldSubProd);
                        db.SubscriptionProducts.Add(newPSubProd);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch (TransactionException ex)
                    {

                        transaction.Dispose();
                    }
                }
            }

        }

        #endregion
    }
}