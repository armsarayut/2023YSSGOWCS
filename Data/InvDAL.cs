using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using GoWMS.Server.Controllers;
using GoWMS.Server.Models;
using GoWMS.Server.Models.Inv;


namespace GoWMS.Server.Data
{
    public class InvDAL
    {
        readonly private string connectionString = ConnGlobals.GetConnLocalDBPG();
        public async Task<IEnumerable<InvStockList>> GetStockList()
        {
    
            List<InvStockList> lstobj = new List<InvStockList>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                StringBuilder Sql = new StringBuilder();
                Sql.AppendLine("select row_number() over(order by  itemcode asc) AS rn,");
                Sql.AppendLine("itemcode, itemname, quantity, pallettag, pallteno, storagearea, storagebin");
                Sql.AppendLine("from wms.inv_stock_go ");
                Sql.AppendLine("order by itemcode");

                NpgsqlCommand cmd = new NpgsqlCommand(Sql.ToString(), con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (await rdr.ReadAsync())
                {
                    InvStockList objrd = new InvStockList
                    {
                        Rn= rdr["rn"] == DBNull.Value ? null : (Int64?)rdr["rn"],
                        Item_code = rdr["itemcode"].ToString(),
                        Item_name = rdr["itemname"].ToString(),
                        Qty = rdr["quantity"] == DBNull.Value ? null : (decimal?)rdr["quantity"],
                        Su_no = rdr["pallettag"].ToString(),
                        Palletcode = rdr["pallteno"].ToString(),
                        Shelfname = rdr["storagebin"].ToString(),
                        StorageArae = rdr["storagearea"].ToString()
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }

        public async Task<IEnumerable<Inv_Stock_GoInfo>> GetStockListInfo()
        {

            List<Inv_Stock_GoInfo> lstobj = new List<Inv_Stock_GoInfo>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                StringBuilder Sql = new StringBuilder();

                //Sql.AppendLine("select row_number() over(order by  itemcode asc) AS rn,");
                //Sql.AppendLine("itemcode, itemname, quantity, pallettag, pallteno, storagearea, storagebin");
                //Sql.AppendLine("from wms.inv_stock_go ");

                //Sql.AppendLine("order by itemcode, ");

             


                Sql.AppendLine("SELECT subQ.*");

                Sql.AppendLine(", CASE WHEN t3.weightnet IS NULL ");
                Sql.AppendLine(" THEN   subQ.quantity");
                Sql.AppendLine(" ELSE subQ.quantity * t3.weightnet ");
                Sql.AppendLine(" END AS disquantity");

                Sql.AppendLine(", CASE WHEN t3.weightnet IS NULL ");
                Sql.AppendLine(" THEN   subQ.totalquantity");
                Sql.AppendLine(" ELSE subQ.totalquantity * t3.weightnet ");
                Sql.AppendLine(" END AS distotalquantity");

                Sql.AppendLine("FROM (");
                Sql.AppendLine("SELECT t1.efidx , t1.efstatus, t1.created, t1.modified, t1.innovator, t1.device");
                Sql.AppendLine(", t1.pono, t1.pallettag, t1.itemtag, t1.itemcode, t1.itemname, t1.itembar, t1.unit");
                Sql.AppendLine(", t1.weightunit, t1.weight, t1.lotno, t1.totalweight");
                Sql.AppendLine(", t1.docno, t1.docby, t1.docdate, t1.docnote, t1.grnrefer, t1.grntime, t1.grtime");
                Sql.AppendLine(", t1.grtype, t1.pallteno, t1.palltmapkey, t1.storagetime, t1.storageno");
                Sql.AppendLine(", t1.storagearea, t2.shelfname as storagebin, t1.gnrefer, t1.allocatequantity, t1.allocateweight");
                Sql.AppendLine(", t1.quantity, t1.totalquantity ");

                Sql.AppendLine("FROM wms.inv_stock_go t1");
                Sql.AppendLine("LEFT JOIN wcs.set_shelf t2");
                Sql.AppendLine("ON t1.pallteno=t2.lpncode");

                Sql.AppendLine("WHERE t1.allocatequantity < t1.quantity");
                Sql.AppendLine(")subQ");

                Sql.AppendLine("LEFT JOIN wms.mas_item_go t3");
                Sql.AppendLine("ON subQ.itemcode=t3.itemcode");


     
                Sql.AppendLine("order by itemcode ASC, docdate ASC, pallettag ASC");


                NpgsqlCommand cmd = new NpgsqlCommand(Sql.ToString(), con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (await rdr.ReadAsync())
                {
                    Inv_Stock_GoInfo objrd = new Inv_Stock_GoInfo
                    {
                        Efidx = rdr["efidx"] == DBNull.Value ? null : (Int64?)rdr["efidx"],
                        Efstatus = rdr["efstatus"] == DBNull.Value ? null : (Int32?)rdr["efstatus"],
                        Created = rdr["created"] == DBNull.Value ? null : (DateTime?)rdr["created"],
                        Modified = rdr["modified"] == DBNull.Value ? null : (DateTime?)rdr["modified"],
                        Innovator = rdr["innovator"] == DBNull.Value ? null : (Int64?)rdr["innovator"],
                        Device = rdr["device"].ToString(),
                        Pono = rdr["pono"].ToString(),
                        Pallettag = rdr["pallettag"].ToString(),
                        Itemtag = rdr["itemtag"].ToString(),
                        Itemcode = rdr["itemcode"].ToString(),
                        Itemname = rdr["itemname"].ToString(),
                        Itembar = rdr["itembar"].ToString(),
                        Unit = rdr["unit"].ToString(),
                        Weightunit = rdr["weightunit"].ToString(),
                        Quantity = rdr["quantity"] == DBNull.Value ? null : (decimal?)rdr["quantity"],
                        Weight = rdr["weight"] == DBNull.Value ? null : (decimal?)rdr["weight"],
                        Lotno = rdr["lotno"].ToString(),
                        Totalquantity = rdr["totalquantity"] == DBNull.Value ? null : (decimal?)rdr["totalquantity"],
                        Totalweight = rdr["totalweight"] == DBNull.Value ? null : (decimal?)rdr["totalweight"],
                        Docno = rdr["docno"].ToString(),
                        Docby = rdr["docby"].ToString(),
                        Docdate = rdr["docdate"] == DBNull.Value ? null : (DateTime?)rdr["docdate"],
                        Docnote = rdr["docnote"].ToString(),
                        Grnrefer = rdr["grnrefer"] == DBNull.Value ? null : (Int64?)rdr["grnrefer"],
                        Grntime = rdr["grntime"] == DBNull.Value ? null : (DateTime?)rdr["grntime"],
                        Grtime = rdr["grtime"] == DBNull.Value ? null : (DateTime?)rdr["grtime"],
                        Grtype = rdr["grtype"].ToString(),
                        Pallteno = rdr["pallteno"].ToString(),
                        Palltmapkey = rdr["palltmapkey"].ToString(),
                        Storagetime = rdr["storagetime"] == DBNull.Value ? null : (DateTime?)rdr["storagetime"],
                        Storageno = rdr["storageno"].ToString(),
                        Storagearea = rdr["storagearea"].ToString(),
                        Storagebin = rdr["storagebin"].ToString(),
                        Gnrefer = rdr["gnrefer"] == DBNull.Value ? null : (Int64?)rdr["gnrefer"],
                        Allocatequantity = rdr["allocatequantity"] == DBNull.Value ? null : (decimal?)rdr["allocatequantity"],
                        Allocateweight = rdr["allocateweight"] == DBNull.Value ? null : (decimal?)rdr["allocateweight"],
                        DisQuantity = rdr["disquantity"] == DBNull.Value ? null : (decimal?)rdr["disquantity"],
                        DisTotalquantity = rdr["distotalquantity"] == DBNull.Value ? null : (decimal?)rdr["distotalquantity"]

                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }



        public IEnumerable<InvStockSum> GetStockSum()
        {
            List<InvStockSum> lstobj = new List<InvStockSum>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                StringBuilder Sql = new StringBuilder();


                Sql.AppendLine("SELECT subQ.* ");
                Sql.AppendLine(",CASE WHEN t3.weightnet IS NULL ");
                Sql.AppendLine("THEN subQ.totalqty ");
                Sql.AppendLine("ELSE subQ.totalqty * t3.weightnet");
                Sql.AppendLine("END AS distotalstock");

                Sql.AppendLine("FROM (");
                Sql.AppendLine("select row_number() over(order by itemcode asc) AS rn,");
                Sql.AppendLine("itemcode, itemname, sum(quantity) as totalqty, count(pallteno) as countpallet, docnote as lot");
                Sql.AppendLine("from wms.inv_stock_go ");
                Sql.AppendLine("WHERE allocatequantity < quantity");
                Sql.AppendLine("group by itemcode, itemname, docnote");
                Sql.AppendLine(")subQ");

                Sql.AppendLine("left join wms.mas_item_go t3");
                Sql.AppendLine("on subQ.itemcode=t3.itemcode");

                Sql.AppendLine("order by rn");

                NpgsqlCommand cmd = new NpgsqlCommand(Sql.ToString(), con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    InvStockSum objrd = new InvStockSum
                    {
                        Rn = rdr["rn"] == DBNull.Value ? null : (Int64?)rdr["rn"],
                        Item_code = rdr["itemcode"].ToString(),
                        Item_name = rdr["itemname"].ToString(),
                        lot = rdr["lot"].ToString(),
                        Totalstock = rdr["totalqty"] == DBNull.Value ? null : (Decimal?)rdr["totalqty"],
                        Countpallet = rdr["countpallet"] == DBNull.Value ? null : (Int64?)rdr["countpallet"],
                        DisTotalstock = rdr["distotalstock"] == DBNull.Value ? null : (Decimal?)rdr["distotalstock"]

                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }




        public IEnumerable<Vrpt_shelf_listInfo> GetShelfLocation()
        {
            List<Vrpt_shelf_listInfo> lstobj = new List<Vrpt_shelf_listInfo>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                 StringBuilder Sql = new StringBuilder();
                //Sql.AppendLine("SELECT t1.modified, t1.srm_no, t1.shelf_no, t1.shelfcode, t1.shelfname");
                //Sql.AppendLine(", t1.shelfbank, t1.shelfframe, t1.shelfbay, t1.shelflevel, t1.shelfstatus");
                //Sql.AppendLine(", t1.lpncode, t1.refercode, t1.actual_weight, t1.actual_size, t1.desc_size, t1.st_desc, t1.backcolor, t1.focecolor");
                //Sql.AppendLine(",t2.pallettag as suno");
                //Sql.AppendLine("from  wcs.vrpt_shelf_list t1");
                //Sql.AppendLine("left join wms.inv_stock_go t2");
                //Sql.AppendLine("On t1.lpncode=t2.pallteno");
                //Sql.AppendLine("order by shelf_no asc");


                Sql.AppendLine("SELECT t1.modified, t1.srm_no, t1.shelf_no, t1.shelfcode, t1.shelfname");
                Sql.AppendLine(", t1.shelfbank, t1.shelfframe, t1.shelfbay, t1.shelflevel, t1.shelfstatus");
                Sql.AppendLine(", t1.lpncode, t1.refercode, t1.actual_weight, t1.actual_size, t1.desc_size, t1.st_desc, t1.backcolor, t1.focecolor");
                Sql.AppendLine(",'-' as suno");
                //Sql.AppendLine(",t2.pallettag as suno");
                Sql.AppendLine("from  wcs.vrpt_shelf_list t1");
                //Sql.AppendLine("left join wms.inv_stock_go t2");
                //Sql.AppendLine("On t1.lpncode=t2.pallteno");
                Sql.AppendLine("order by shelf_no asc");


                NpgsqlCommand cmd = new NpgsqlCommand(Sql.ToString(), con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Vrpt_shelf_listInfo objrd = new Vrpt_shelf_listInfo
                    {
                       
                        Srm_no = rdr["srm_no"] == DBNull.Value ? null : (Int32?)rdr["srm_no"],
                        Shelf_no = rdr["shelf_no"] == DBNull.Value ? null : (Int32?)rdr["shelf_no"],
                        Shelfcode = rdr["shelfcode"].ToString(),
                        Shelfname  = rdr["shelfname"].ToString(),
                        Shelfbank = rdr["shelfbank"] == DBNull.Value ? null : (Int16?)rdr["shelfbank"],
                        Shelfbay = rdr["shelfbay"] == DBNull.Value ? null : (Int32?)rdr["shelfbay"],
                        Shelfframe = rdr["shelfframe"] == DBNull.Value ? null : (Int16?)rdr["shelfframe"],
                        Shelflevel = rdr["shelflevel"] == DBNull.Value ? null : (Int16?)rdr["shelflevel"],
                        Shelfstatus = rdr["shelfstatus"] == DBNull.Value ? null : (Int32?)rdr["shelfstatus"],
                        Lpncode= rdr["lpncode"].ToString(),
                        Refercode = rdr["refercode"].ToString(),
                        Actual_weight = rdr["actual_weight"] == DBNull.Value ? null : (decimal?)rdr["actual_weight"],
                        Actual_size = rdr["actual_size"] == DBNull.Value ? null : (Int32?)rdr["actual_size"],
                        Desc_size = rdr["desc_size"].ToString(),
                        St_desc = rdr["st_desc"].ToString(),
                        Modified = rdr["modified"] == DBNull.Value ? null : (DateTime?)rdr["modified"],
                        Backcolor = rdr["backcolor"].ToString(),
                        Focecolor = rdr["focecolor"].ToString(),
                        Suno= rdr["suno"].ToString()

                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }



        public IEnumerable<InvStockSumByCus> GetStockSumByCustomer()
        {
            List<InvStockSumByCus> lstobj = new List<InvStockSumByCus>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                StringBuilder Sql = new StringBuilder();

                Sql.AppendLine("SELECT t1.itemcode, t1.itemname, t1.palltmapkey, t2.cus_name, t1.pallteno, t1.storagebin, sum(t1.quantity) as totalstock, t1.itembar");
                Sql.AppendLine(",t3.srm_no, t3.shelfbank, t3.shelfbay, t3.shelflevel");
                Sql.AppendLine("FROM wms.inv_stock_go t1");
                Sql.AppendLine("LEFT JOIN public.sap_customermaster_v t2");
                Sql.AppendLine("ON t1.palltmapkey= t2.cus_code");
                Sql.AppendLine("LEFT JOIN wcs.set_shelf t3");
                Sql.AppendLine("ON t1.storagebin = t3.shelfcode");
                Sql.AppendLine("WHERE t1.allocatequantity < t1.quantity");
                Sql.AppendLine("GROUP BY t1.itemcode, t1.itemname, t1.palltmapkey,t1. pallteno, t1.storagebin,  t2.cus_name, t3.srm_no, t3.shelfbank, t3.shelfbay, t3.shelflevel, t1.itembar");
                
                //Sql.AppendLine("ORDER BY t1.itemcode");


                NpgsqlCommand cmd = new NpgsqlCommand(Sql.ToString(), con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    InvStockSumByCus objrd = new InvStockSumByCus
                    {
                        Itemcode = rdr["itemcode"].ToString(),
                        Itemname = rdr["itemname"].ToString(),
                        Cuscode = rdr["palltmapkey"].ToString(),
                        Cusname = rdr["cus_name"].ToString(),
                        Pallteno = rdr["itembar"].ToString(),
                        Storagebin = rdr["storagebin"].ToString(),
                        Totalstock = rdr["totalstock"] == DBNull.Value ? null : (Decimal?)rdr["totalstock"],
                        StorageLane = rdr["srm_no"] == DBNull.Value ? null : (Int32?)rdr["srm_no"],
                        StorageBank = rdr["shelfbank"] == DBNull.Value ? null : (Int16?)rdr["shelfbank"],
                        StorageBay = rdr["shelfbay"] == DBNull.Value ? null : (Int32?)rdr["shelfbay"],
                        StorageLevel = rdr["shelflevel"] == DBNull.Value ? null : (Int16?)rdr["shelflevel"],
                        Palltego = rdr["pallteno"].ToString()
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }




    }
}
