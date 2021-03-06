﻿namespace EarlySite.Drms.Spefication
{
    using System.Text;
    using System.Collections.Generic;
    using Model.Database;

    public class RelationShareAddSpefication : SpeficationBase
    {
        private IList<RelationShareInfo> _info;

        /// <summary>
        /// 分享关系契约
        /// </summary>
        /// <param name="shareinfo"></param>
        public RelationShareAddSpefication(IList<RelationShareInfo> shareinfo)
        {
            _info = shareinfo;
        }

        public override string Satifasy()
        {
            StringBuilder sql = new StringBuilder();

            if(_info != null && _info.Count > 0)
            {
                sql.Append("insert into relation_share (RecipesId,DishId,Phone,UpdateDate) values ");

                for (int i = 0; i < _info.Count; i++)
                {
                    sql.AppendFormat("('{0}','{1}','{2}','{3}')", _info[i].RecipesId, _info[i].DishId, _info[i].Phone, _info[i].UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    if(i != _info.Count - 1)
                    {
                        sql.Append(" , ");
                    }
                }
            }
            return sql.ToString();
        }
    }
}
