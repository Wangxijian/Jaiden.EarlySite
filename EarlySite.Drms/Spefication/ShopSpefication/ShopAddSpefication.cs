﻿namespace EarlySite.Drms.Spefication
{
    using EarlySite.Model.Database;

    public class ShopAddSpefication : SpeficationBase
    {
        private ShopInfo _Info;

        public ShopAddSpefication(ShopInfo shopInfo)
        {
            _Info = shopInfo;
        }

        public override string Satifasy()
        {
            string sql = string.Format(" insert ShopId,ShopName,Longitude,Latitude,UpdateDate,Descripion into which_shop values ('{0}','{1}','{2}','{3}','{4}')", _Info.ShopId, _Info.ShopName, _Info.Longitude, _Info.Latitude, _Info.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"), _Info.Description);
            return sql;
        }
    }
}
