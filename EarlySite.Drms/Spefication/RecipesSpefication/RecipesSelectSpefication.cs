﻿namespace EarlySite.Drms.Spefication
{
    using System.Text;
    using EarlySite.Model.Database;

    public class RecipesSelectSpefication : SpeficationBase
    {
        private RecipesInfo _info;
        private int _type = 0;

        /// <summary>
        /// 食谱查询规约
        /// </summary>
        /// <param name="recipesInfo">食谱信息</param>
        /// <param name="type">
        /// 查询类型
        /// 0:根据编号查询
        /// 1:根据食谱名称模糊查询
        /// </param>
        public RecipesSelectSpefication(RecipesInfo recipesInfo,int type)
        {
            _info = recipesInfo;
            _type = type;
        }

        public override string Satifasy()
        {
            StringBuilder sb = new StringBuilder();

            if(_type == 0)
            {
                sb.AppendFormat(" select RecipesId,Name,UpdateDate,Phone,Cover,Description,Tag,IsPrivate from which_recipes where RecipesId = '{0}' ", _info.RecipesId);
            }
            if(_type == 1)
            {
                sb.AppendFormat(" select RecipesId,Name,UpdateDate,Phone,Cover,Description,Tag,IsPrivate from which_recipes where Name like '%{0}%' ", _info.Name);
            }


            return sb.ToString();
        }
    }
}