using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIS.Models;
using WebCommonFunction;

namespace MIS.Controllers
{
    public class HomeController : Controller
    {
        tncinformationliveEntities datas = new tncinformationliveEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getFind(string itemcode = "", string jobno = "", string dtFrom = "", string dtTo = "", string wc = "", string plant = "")
        {
            var sql = datas.td_information_data.Where(w => w.job_status == "0");

            if (itemcode == "" || dtFrom == "" || dtTo == "" || jobno == "" || wc == "" || plant == "")
            {
                sql = null;
            }
            else
            {
                if (itemcode != "")
                {
                    sql = sql.Where(w =>
                        w.finished_goods_code.Contains(itemcode) ||
                        w.mc1_item.Contains(itemcode) ||
                        w.mc2_item.Contains(itemcode) ||
                        w.sp1_item.Contains(itemcode) ||
                        w.sp2_item.Contains(itemcode));
                } // end if itemcode not null
                if (dtFrom != "" && dtTo != "")
                {
                    sql = sql.Where(w => w.curing_date.CompareTo(dtFrom) >= 0 && w.curing_date.CompareTo(dtTo) <= 0);
                } // end if dateS not null
                if (jobno != "")
                {
                    sql = sql.Where(w => w.job_order_no.Contains(jobno));
                } // end if jobno not null
                if (wc != "")
                {
                    sql = sql.Where(w => w.wc.Contains(wc));
                } // end if wc not null
                if (plant != "")
                {
                    sql = sql.Where(w => w.plant == plant);
                }// end if plant not null
            }
            return Json(sql.Select(s => new { itemcode = s.finished_goods_code, jobno = s.job_order_no, totalwip = s.total_wip, lastprocess = s.last_process, wc = s.wc, plant = s.plant }));

        } // end ActionResult getFind

        public ActionResult _ShowTable(string itemcode = "", string jobno = "", string dtFrom = "", string dtTo = "", string wc = "", string plant = "")
        {
            var sql = datas.td_information_data.Where(w => w.job_status == "0");

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.Contains(itemcode) || w.mc1_item.Contains(itemcode) || w.mc2_item.Contains(itemcode) || w.sp1_item.Contains(itemcode) || w.sp2_item.Contains(itemcode));
            } // end if itemcode not null

            if (dtFrom != "" && dtTo != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(dtFrom) >= 0 && w.curing_date.CompareTo(dtTo) <= 0);
            } // end if jobno not null

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.Contains(jobno));
            } // end if jobno not null

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.Contains(wc));
            } // end if jobno not null

            if (plant != "")
            {
                if (plant == "FTD")
                {
                    sql = sql.Where(w => w.plant == "FTD" || w.plant == "FTH");
                }
                else
                {
                    sql = sql.Where(w => w.plant == plant);
                }

            } // end if jobno not null

            ViewBag.Inform = sql;//.Take(200);
            ViewBag.SumWip = sql.Where(w => w.total_wip != null).Sum(s => s.total_wip.Value);

            return PartialView();
        }

        public void ExportMain(string itemcode = "", string jobno = "", string dtFrom = "", string dtTo = "", string wc = "", string plant = "")
        {
            TNCUtility util = new TNCUtility();
            var sql = datas.td_information_data.Where(w => w.job_status == "0");

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.Contains(itemcode) || w.mc1_item.Contains(itemcode) || w.mc2_item.Contains(itemcode) || w.sp1_item.Contains(itemcode) || w.sp2_item.Contains(itemcode));
            } // end if itemcode not null

            if (dtFrom != "" && dtTo != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(dtFrom) >= 0 && w.curing_date.CompareTo(dtTo) <= 0);
            } // end if jobno not null

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.Contains(jobno));
            } // end if jobno not null

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.Contains(wc));
            } // end if jobno not null

            if (plant != "")
            {
                if (plant == "FTD")
                {
                    sql = sql.Where(w => w.plant == "FTD" || w.plant == "FTH");
                }
                else
                {
                    sql = sql.Where(w => w.plant == plant);
                }

            } // end if jobno not null

            var output = sql
                    .Select(s => new
                    {
                        JobNo = s.job_order_no,
                        TotalWip = s.total_wip,
                        LastProcess = s.last_process,
                        Plant = s.plant.Trim() == "FTB" ? "OSP1" : (s.plant.Trim() == "FTC" ? "OSP2" : (s.plant.Trim() == "FTD" || s.plant.Trim() == "FTH" ? "RSP" : (s.plant.Trim() == "FTG" ? "BPK" : (s.plant.Trim() == "FTE" ? "PTN" : (s.plant.Trim() == "FTF" ? "MIXING" : (s.plant.Trim() == "FTI" ? "BPK3" : "")))))),
                        WC = s.wc + "/" + s.machine_no,
                        Item = s.finished_goods_code,
                        CuringDate = s.curing_date,
                        LotNo = s.lot_no,
                        CuringQty = s.curing_qty,
                        GoodItemQty = s.good_item_qty,
                        CuringStartDate = s.curing_start_date,
                        WipCuringQty = s.wip_curing_qty,
                        PostcureStartDate = s.post_start_date,
                        GreasingDate = s.greasing_date,
                        PackingDate = s.packing_date,
                        WipPackingQty = s.wip_packing_qty,
                        RB1Compound = s.rb1_compound,
                        RB1Weight = s.rb1_weight,
                        RB1StockInDate = s.rb1_stock_in_date,
                        RB1BatchNo1 = s.rb1_batch_no1,
                        RB1BatchNo2 = s.rb1_batch_no2,
                        RB1BatchNo3 = s.rb1_batch_no3,
                        RB1BatchNo4 = s.rb1_batch_no4,
                        RB2Compound = s.rb2_compound,
                        RB2Weight = s.rb2_weight,
                        RB2StockInDate = s.rb2_stock_in_date,
                        RB2BatchNo1 = s.rb2_batch_no1,
                        RB2BatchNo2 = s.rb2_batch_no2,
                        RB2BatchNo3 = s.rb2_batch_no3,
                        RB2BatchNo4 = s.rb2_batch_no4,
                        MC1Item = s.mc1_item,
                        MC1Qty = s.mc1_qty,
                        MC1Process = s.mc1_process,
                        MC2Item = s.mc2_item,
                        MC2Qty = s.mc2_qty,
                        MC2Process = s.mc2_process,
                        SP1Item = s.sp1_item,
                        SP1Qty = s.sp1_qty,
                        SP1Process = s.sp1_process,
                        SP2Item = s.sp2_item,
                        SP2Qty = s.sp2_qty,
                        Sp2Process = s.sp2_process
                    });//.Take(200);
                   
            util.CreateExcel(output.ToList(), "Export");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
