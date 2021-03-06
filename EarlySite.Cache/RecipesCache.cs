﻿namespace EarlySite.Cache
{
    using System;
    using Model.Database;
    using EarlySite.Cache.CacheBase;
    using System.Collections.Generic;
    using EarlySite.Drms.DBManager;
    using EarlySite.Drms.Spefication;
    using Core.Utils;
    /// <summary>
    /// 食谱信息缓存
    /// <!--Redis Key格式-->
    /// DB_RI_食谱编号_手机号
    /// </summary>
    public partial class RecipesCache : IRecipesCache
    {
        /// <summary>
        /// 设置食谱禁启用状态
        /// </summary>
        /// <param name="recipesId">食谱编号</param>
        /// <param name="enable">
        /// true:启用
        /// false:禁用
        /// </param>
        /// <returns></returns>
        public bool SetRecipesEnable(int recipesId, bool enable)
        {
            if(recipesId == 0)
            {
                throw new ArgumentNullException("recipesId can not be zero");
            }
            bool result = false;

            string key = string.Format("DB_RI_{0}_*", recipesId);

            RecipesInfo updateinfo = null;
            IList<string> keys = Session.Current.ScanAllKeys(key);
            if (keys != null && keys.Count > 0)
            {
                updateinfo = Session.Current.Get<RecipesInfo>(keys[0]);
            }
            if(updateinfo != null)
            {
                updateinfo.Enable = enable;
                //移除之前的
                Session.Current.Remove(keys[0]);
                result = Session.Current.Set(updateinfo.GetKeyName(), updateinfo);
            }
            return result;
        }


        /// <summary>
        /// 设置食谱禁用(设置当前用户全部食谱)
        /// </summary>
        /// <param name="phone"></param>
        public void SetRecipesEnable(long phone,bool enable)
        {
            if (phone == 0)
            {
                throw new ArgumentNullException("phone can not be zero");
            }
            string key = string.Format("DB_RI_*_{0}", phone);
            IList<string> keys = Session.Current.ScanAllKeys(key);
            if (keys != null && keys.Count > 0)
            {
                RecipesInfo updateinfo = null;
                for (int i = 0; i < keys.Count; i++)
                {
                    updateinfo = Session.Current.Get<RecipesInfo>(keys[i]);
                }
                if (updateinfo != null)
                {
                    updateinfo.Enable = enable;
                    Session.Current.Set(updateinfo.GetKeyName(), updateinfo);
                }
            }
        }

        /// <summary>
        /// 根据手机号获取喜爱的食谱集
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        IList<RecipesInfo> IRecipesCache.GetRecipesInfoById(IList<int> recipesIds)
        {
            IList<RecipesInfo> result = null;

            if(recipesIds != null && recipesIds.Count > 0)
            {
                result = new List<RecipesInfo>();
                foreach (int id in recipesIds)
                {
                    RecipesInfo model = GetRecipesInfoById(id);
                    if(model != null)
                    {
                        result.Add(model);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据食谱编号获取缓存
        /// </summary>
        /// <param name="recipesId"></param>
        /// <returns></returns>
        public RecipesInfo GetRecipesInfoById(int recipesId)
        {
            string key = string.Format("DB_RI_{0}_*", recipesId);

            RecipesInfo result = null;
            IList<string> keys = Session.Current.ScanAllKeys(key);
            if (keys != null && keys.Count > 0)
            {
                result = Session.Current.Get<RecipesInfo>(keys[0]);
            }
            if (result == null)
            {

                //从数据库获取数据
                IList<RecipesInfo> recipeslist = DBConnectionManager.Instance.Reader.Select<RecipesInfo>(new RecipesSelectSpefication(recipesId.ToString(), 0).Satifasy());

                if (recipeslist != null && recipeslist.Count > 0)
                {
                    //更新缓存
                    result = recipeslist[0];
                    Session.Current.Set(result.GetKeyName(), result);
                    Session.Current.Expire(result.GetKeyName(), ExpireTime);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据手机号获食谱集
        /// </summary>
        /// <param name="recipesId"></param>
        /// <returns></returns>
        IList<RecipesInfo> IRecipesCache.GetRecipesInfoByPhone(long phone)
        {
            string key = string.Format("DB_RI_*_{0}", phone);

            IList<RecipesInfo> result = null;
            IList<string> keys = Session.Current.ScanAllKeys(key);
            if (keys != null && keys.Count > 0)
            {
                result = Session.Current.GetAll<RecipesInfo>(keys).ToList<RecipesInfo>();
            }
            if (result == null)
            {

                //从数据库获取数据
                result = DBConnectionManager.Instance.Reader.Select<RecipesInfo>(new RecipesSelectSpefication(phone.ToString(), 2).Satifasy());

                if (result != null && result.Count > 0)
                {
                    //更新缓存
                    IDictionary<string, RecipesInfo> dictionary = new Dictionary<string, RecipesInfo>();
                    foreach (RecipesInfo item in result)
                    {
                        dictionary.Add(item.GetKeyName(), item);
                    }
                    Session.Current.SetAll<RecipesInfo>(dictionary);
                }
            }
            return result;
        }
    }


    /// <summary>
    /// 食谱信息缓存
    /// <!--Redis Key格式-->
    /// DB_RI_食谱编号_手机号
    /// </summary>
    public partial class RecipesCache : IRecipesCache
    {
        /// <summary>
        /// 有效时间
        /// </summary>
        public const int EffectiveTime = 15;


        /// <summary>
        /// 失效时间
        /// </summary>
        public static DateTime ExpireTime { get { return DateTime.Now.AddDays(EffectiveTime); } }


        void ICache<RecipesInfo>.LoadCache()
        {
            throw new NotImplementedException();
        }

        bool ICache<RecipesInfo>.RemoveInfo(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key can not be null");
            }
            bool issuccess = false;
            if (Session.Current.Contains(key))
            {
                issuccess = Session.Current.Remove(key);
            }
            return issuccess;
        }

        bool ICache<RecipesInfo>.RemoveInfo(RecipesInfo param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("Recipes info can not be null");
            }
            string key = param.GetKeyName();
            bool issuccess = false;
            issuccess = Session.Current.Remove(key);
            return issuccess;
        }

        bool ICache<RecipesInfo>.SaveInfo(RecipesInfo param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("Recipes info can not be null");
            }
            string key = param.GetKeyName();
            bool issuccess = false;
            issuccess = Session.Current.Set(key, param);
            Session.Current.Expire(key, ExpireTime);
            return issuccess;
        }

        RecipesInfo ICache<RecipesInfo>.SearchInfoByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("search key can not be null");
            }

            RecipesInfo result = null;
            IList<string> keys = Session.Current.ScanAllKeys(key);
            if (keys != null && keys.Count > 0)
            {
                result = Session.Current.Get<RecipesInfo>(keys[0]);
            }
            return result;
        }
    }
}
