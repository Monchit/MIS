using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIS.Models;
using WebCommonFunction;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Drawing;

namespace MIS.Controllers
{
    public class HomeController : Controller
    {
        tncinformationliveEntities datas = new tncinformationliveEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index1()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult WIP()
        {
            return View();
        }

        public ActionResult WIP2()
        {
            return View();
        }

        public JsonResult GetDataDetail(string itemcode = "", string jobno = "", string st_cur = "", string fn_cur = "", string wc = "", string mc = "", string plant = "", string laststatus = "-")
        {
            var sql = datas.td_information_data.Where(w => w.job_status == "0" && w.total_wip != null);

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.ToUpper().Contains(itemcode.ToUpper()));
            }

            if (st_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(st_cur) >= 0);
            }

            if (fn_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(fn_cur) <= 0);
            }

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.ToUpper().Contains(jobno.ToUpper()));
            }

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.ToUpper() == wc.ToUpper());//w.wc.ToUpper().Contains(wc.ToUpper())
            }

            if (mc != "")
            {
                sql = sql.Where(w => w.machine_no == mc);//w.machine_no.ToUpper().Contains(mc.ToUpper())
            }

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
            }

            if (laststatus != "-")
            {
                sql = sql.Where(w => w.last_process.Trim() == laststatus);
            }

            var result = sql.Select(s => new
            {
                s.job_order_no,
                s.total_wip,
                s.last_process,
                s.plant,
                wc = s.wc + "/" + s.machine_no,
                s.finished_goods_code,
                curing_date = s.curing_date,//.Insert(4,"-").Insert(7,"-"),
                s.lot_no,
                s.curing_qty,
                s.good_item_qty,
                curing_start_date = s.curing_start_date,//.Insert(4,"-").Insert(7,"-"),
                s.wip_curing_qty,
                post_start_date = s.post_start_date,//.Insert(4,"-").Insert(7,"-"),
                greasing_date = s.greasing_date,//.Insert(4,"-").Insert(7,"-"),
                packing_date = s.packing_date,//.Insert(4,"-").Insert(7,"-"),
                s.wip_packing_qty,
                s.rb1_compound,
                s.rb1_weight,
                rb1_stock_in_date = s.rb1_stock_in_date,//.Insert(4,"-").Insert(7,"-"),
                s.rb1_batch_no1,
                s.rb1_batch_no2,
                s.rb1_batch_no3,
                s.rb1_batch_no4,
                s.rb2_compound,
                s.rb2_weight,
                s.rb2_stock_in_date,
                s.rb2_batch_no1,
                s.rb2_batch_no2,
                s.rb2_batch_no3,
                s.rb2_batch_no4,
                s.mc1_item,
                s.mc1_qty,
                s.mc1_process,
                s.mc2_item,
                s.mc2_qty,
                s.mc2_process,
                s.sp1_item,
                s.sp1_qty,
                s.sp1_process,
                s.sp2_item,
                s.sp2_qty,
                s.sp2_process
            }).Take(500).ToList(); // <--- cast to list if GetUserContacts returns an IEnumerable
            return Json(result, JsonRequestBehavior.AllowGet);
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

        public ActionResult _ShowTable(string itemcode = "", string jobno = "", string st_cur = "", string fn_cur = "", string wc = "", string mc = "", string plant = "", string laststatus = "-")
        {
            var sql = datas.td_information_data.Where(w => w.job_status == "0" && w.total_wip != null);

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.ToUpper().Contains(itemcode.ToUpper()));
            }

            if (st_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(st_cur) >= 0 );
            }

            if(fn_cur != ""){
                sql = sql.Where(w => w.curing_date.CompareTo(fn_cur) <= 0);
            }

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.ToUpper().Contains(jobno.ToUpper()));
            }

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.ToUpper() == wc.ToUpper());//w.wc.ToUpper().Contains(wc.ToUpper())
            }

            if (mc != "")
            {
                sql = sql.Where(w => w.machine_no == mc);//w.machine_no.ToUpper().Contains(mc.ToUpper())
            }

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
            }

            if (laststatus != "-")
            {
                sql = sql.Where(w => w.last_process.Trim() == laststatus);

                ViewBag.TotalWIP = sql.Sum(s => s.total_wip.Value);
                ViewBag.Exjob = sql.Where(w => w.last_process.Trim() != "Plan").Sum(s => s.total_wip.Value);
                ViewBag.Plan = sql.Where(w => w.last_process.Trim() == "Plan").Sum(s => s.total_wip.Value);
                ViewBag.Cur = sql.Where(w => w.last_process.Trim() == "Curing").Sum(s => s.total_wip.Value);
                ViewBag.AfterCur = sql.Where(w => w.last_process.Trim() == "After Cured").Sum(s => s.total_wip.Value);
                ViewBag.Packing = sql.Where(w => w.last_process.Trim() == "Packing").Sum(s => s.total_wip.Value);
                ViewBag.AfterPack = sql.Where(w => w.last_process.Trim() == "After Packed").Sum(s => s.total_wip.Value);
            }
            else
            {
                ViewBag.TotalWIP = sql.Sum(s => s.total_wip.Value);
                ViewBag.Exjob = sql.Where(w => w.last_process.Trim() != "Plan").Sum(s => s.total_wip.Value);
                ViewBag.Plan = sql.Where(w => w.last_process.Trim() == "Plan").Sum(s => s.total_wip.Value);
                ViewBag.Cur = sql.Where(w => w.last_process.Trim() == "Curing").Sum(s => s.total_wip.Value);
                ViewBag.AfterCur = sql.Where(w => w.last_process.Trim() == "After Cured").Sum(s => s.total_wip.Value);
                ViewBag.Packing = sql.Where(w => w.last_process.Trim() == "Packing").Sum(s => s.total_wip.Value);
                ViewBag.AfterPack = sql.Where(w => w.last_process.Trim() == "After Packed").Sum(s => s.total_wip.Value);
            }

            ViewBag.Inform = sql.Take(500);

            return PartialView();
        }

        public ActionResult _ShowTable1(string itemcode = "", string jobno = "", string st_cur = "", string fn_cur = "", string wc = "", string mc = "", string plant = "", string laststatus = "-")
        {
            var sql = datas.td_information_data.Where(w => w.job_status == "0" && w.total_wip != null);

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.Contains(itemcode));
                //sql = sql.Where(w => w.finished_goods_code.ToUpper().Contains(itemcode.ToUpper()));
            }

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.Contains(jobno));
                //sql = sql.Where(w => w.job_order_no.ToUpper().Contains(jobno.ToUpper()));
            }

            if (wc != "")
            {
                sql = sql.Where(w => w.wc == wc);
                //sql = sql.Where(w => w.wc.ToUpper() == wc.ToUpper());//w.wc.ToUpper().Contains(wc.ToUpper())
            }

            if (mc != "")
            {
                sql = sql.Where(w => w.machine_no == mc);//w.machine_no.ToUpper().Contains(mc.ToUpper())
            }

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
            }

            if (st_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(st_cur) >= 0);
            }

            if (fn_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(fn_cur) <= 0);
            }

            if (laststatus != "-")
            {
                sql = sql.Where(w => w.last_process == laststatus);
                //sql = sql.Where(w => w.last_process.Trim() == laststatus);
            }

            if (wc != "" && mc != "" && plant != "")
            {
                var target = datas.tm_target.Where(w => w.plant == plant && w.machine_no == mc && w.wc == wc).FirstOrDefault();
                var curing = sql.Where(w => w.last_process.Trim() == "Curing").FirstOrDefault();
                if (target != null && curing != null)
                {
                    var cal_target = curing.curing_qty.Value * target.stock_day.Value * target.lot_per_day.Value;

                    ViewBag.CalTarget = cal_target;
                }
                else
                {
                    ViewBag.CalTarget = 0;
                }
            }
            else
            {
                ViewBag.CalTarget = 0;
            }

            //sql = sql.Take(500);

            return PartialView(sql);
        }

        public ActionResult _ShowTable2(string itemcode = "", string jobno = "", string st_cur = "", string fn_cur = "", string wc = "", string mc = "", string plant = "", string laststatus = "-")
        {
            var sql = datas.td_information_data.Where(w => w.job_status == "0" && w.total_wip != null);

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.ToUpper().Contains(itemcode.ToUpper()));
            }

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.ToUpper().Contains(jobno.ToUpper()));
            }

            if (st_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(st_cur) >= 0);
            }

            if (fn_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(fn_cur) <= 0);
            }

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.ToUpper() == wc.ToUpper());//w.wc.ToUpper().Contains(wc.ToUpper())
            }

            if (mc != "")
            {
                sql = sql.Where(w => w.machine_no == mc);
            }

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
            }

            if (laststatus != "-")
            {
                sql = sql.Where(w => w.last_process.Trim() == laststatus);
            }

            if (wc != "" && mc != "" && plant != "")
            {
                var target = datas.tm_target.Where(w => w.plant == plant && w.machine_no == mc && w.wc.ToUpper() == wc.ToUpper()).FirstOrDefault();
                var curing = sql.Where(w => w.last_process.Trim() == "Curing").FirstOrDefault();
                if (target != null && curing != null)
                {
                    var cal_target = curing.curing_qty.Value * target.stock_day.Value * target.lot_per_day.Value;
                    ViewBag.CalTarget = cal_target;
                }
                else
                {
                    ViewBag.CalTarget = 0;
                }
            }
            else
            {
                ViewBag.CalTarget = 0;
            }

            return PartialView();
        }

        public void ExportMain(string itemcode = "", string jobno = "", string st_cur_dt = "", string fn_cur_dt = "", string wc = "", string mc = "", string plant = "", string laststatus = "-")
        {
            TNCUtility util = new TNCUtility();
            var sql = datas.td_information_data.Where(w => w.job_status == "0" && w.total_wip != null);

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.ToUpper().Contains(itemcode.ToUpper()));
            }

            if (st_cur_dt != "" && fn_cur_dt != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(st_cur_dt) >= 0 && w.curing_date.CompareTo(fn_cur_dt) <= 0);
            }

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.Contains(jobno));
            }

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.ToUpper() == wc.ToUpper());
            }

            if (mc != "")
            {
                sql = sql.Where(w => w.machine_no == mc);
            }

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

            }

            if (laststatus != "-")
            {
                sql = sql.Where(w => w.last_process.Trim() == laststatus);
            }

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

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult _GetGraphWIP(string itemcode = "", string jobno = "", string st_cur = "", string fn_cur = "", string wc = "", string mc = "", string plant = "", string laststatus = "-")
        {
            Highcharts chart = new Highcharts("chart")
            .InitChart(new Chart
            {
                PlotShadow = false,
                Height = 350
            })
            .SetTitle(new Title { Text = "Process Share" })
            .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ Highcharts.numberFormat(this.y,0,'',',') +''; }" })
            .SetPlotOptions(new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    AllowPointSelect = true,
                    Cursor = Cursors.Pointer,
                    ShowInLegend = true,
                    DataLabels = new PlotOptionsPieDataLabels
                    {
                        Color = ColorTranslator.FromHtml("#000000"),
                        ConnectorColor = ColorTranslator.FromHtml("#000000"),
                        Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ Highcharts.numberFormat(this.percentage, 2) +' %'; }"
                    }
                }
            })
            .SetOptions(new GlobalOptions
            {
                Colors = new[]{
                    ColorTranslator.FromHtml("#FFA07A"),
                    ColorTranslator.FromHtml("#ECEE00"),
                    ColorTranslator.FromHtml("#66CDAA"),
                    ColorTranslator.FromHtml("#00FFFF"),
                    ColorTranslator.FromHtml("#00FF90")
                }
            })
            .SetCredits(new Credits { Enabled = false });

            var sql = datas.td_information_data.Where(w => w.job_status == "0" && w.total_wip != null);

            if (itemcode != "")
            {
                sql = sql.Where(w => w.finished_goods_code.ToUpper().Contains(itemcode.ToUpper()));
            }

            if (st_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(st_cur) >= 0);
            }

            if (fn_cur != "")
            {
                sql = sql.Where(w => w.curing_date.CompareTo(fn_cur) <= 0);
            }

            if (jobno != "")
            {
                sql = sql.Where(w => w.job_order_no.ToUpper().Contains(jobno.ToUpper()));
            }

            if (wc != "")
            {
                sql = sql.Where(w => w.wc.ToUpper() == wc.ToUpper());//w.wc.ToUpper().Contains(wc.ToUpper())
            }

            if (mc != "")
            {
                sql = sql.Where(w => w.machine_no == mc);//w.machine_no.ToUpper().Contains(mc.ToUpper())
            }

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
            }

            var data_wip = new List<object[]>();

            if (laststatus != "-")
            {
                sql = sql.Where(w => w.last_process.Trim() == laststatus);

                data_wip.Add(new object[] { "Plan", sql.Where(w => w.last_process.Trim() == "Plan").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "Curing", sql.Where(w => w.last_process.Trim() == "Curing").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "After Cured", sql.Where(w => w.last_process.Trim() == "After Cured").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "Packing", sql.Where(w => w.last_process.Trim() == "Packing").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "After Packed", sql.Where(w => w.last_process.Trim() == "After Packed").Sum(s => s.total_wip.Value) });
            }
            else
            {
                data_wip.Add(new object[] { "Plan", sql.Where(w => w.last_process.Trim() == "Plan").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "Curing", sql.Where(w => w.last_process.Trim() == "Curing").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "After Cured", sql.Where(w => w.last_process.Trim() == "After Cured").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "Packing", sql.Where(w => w.last_process.Trim() == "Packing").Sum(s => s.total_wip.Value) });
                data_wip.Add(new object[] { "After Packed", sql.Where(w => w.last_process.Trim() == "After Packed").Sum(s => s.total_wip.Value) });
            }

            chart.SetSeries(new[]
            {
                new Series { Type = ChartTypes.Pie, Name = "Share", Data = new Data(data_wip.ToArray()) }
            });

            return PartialView(chart);
        }

        public ActionResult BI()
        {
            return View();
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
