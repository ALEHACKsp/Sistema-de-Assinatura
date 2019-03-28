using Memberships.Entities;
using Memberships.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Memberships.Extensions
{
    public static class SubscriptionExtensions
    {
        public static async Task<int> GetSubscriptionIdbyRegistrationCode(this IDbSet<Subscription> subscription, string code)
        {
            try
            {
                if (subscription == null || code == null || code.Length <= 0)
                    return Int32.MinValue;

                var subscriptionId = await (from s in subscription
                                            where s.RegistrationCode.Equals(code)
                                            select s.Id).FirstOrDefaultAsync();
                return subscriptionId;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static async Task Register(this IDbSet<UserSubscription> userSubscriptions, int subscriptionId,string userId)
        {
            try
            {
                if (userSubscriptions == null || subscriptionId.Equals(Int32.MinValue) || userId.Equals(string.Empty))
                    return;

                var exist = await Task.Run(() =>
                userSubscriptions.CountAsync(s => s.SubscriptionId.Equals(subscriptionId) && s.UserId.Equals(userId))) > 0;

                if (!exist)
                    await Task.Run(() =>
                    userSubscriptions.Add(new UserSubscription
                    {

                        UserId = userId,
                        SubscriptionId = subscriptionId,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.MaxValue
                    }));
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public static async Task<bool> RegisterUserSubscriptionCode(string userId, string code)
        {
            try
            {
                var db = ApplicationDbContext.Create();

                if (userId.Equals(string.Empty) || code.Equals(string.Empty))
                    return false;

                var id = await db.Subscriptions.GetSubscriptionIdbyRegistrationCode(code);
                if (id <= 0) return false;

                await db.UserSubscriptions.Register(id, userId);

                if (db.ChangeTracker.HasChanges())
                    await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}