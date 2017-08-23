using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AngleSharp;
using System.Data.SqlClient;
using System.Configuration;
using AngleSharp.Dom.Html;
using System.Runtime.Remoting.Contexts;

namespace Database
{
    public class Coure_db
    {
        public async void Lecturer_info()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);


            List<string> courses_list = new List<string>(new string[]
            {"AKM","ATA","ALM","BEB","BED","BEN","BIL","BIO","BLG","BLS","BUS","CAB","CEV","CHE","CHZ","CIE","CMP",
            "COM","DEN","DFH", "DGH","DNK","DUI","EAS","ECO","ECM","EHA","EHB","EHN","EKO","ELE","ELH","ELK","ELT",
            "END","ENE","ENG","ENR","ESL","ESM","ETK","EUT","FIZ","FRA","FZK","GED","GEM","GEO","GID","GLY","GMI",
            "GMK","GSB","GSL","GUV","GVT","HUK","HSS","ICM","ILT", "IML","ING","IMS","ISE", "ISH","ISL","ISP","ITA",
            "ITB","JDF","JEF", "JEO","JPN","KIM","KMM","KMP","KON","LAT","MAD", "MAK","MAL","MAT","MEK","MEN","MET",
            "MCH","MIM","MKN","MST","MTM","MOD","MRE","MRT","MTH","MTK","MTO","MTR","MUH","MUK","MUT","MUZ","NAE",
            "NTH","PAZ","PEM","PME","PET","PHE","PHY","RES","RUS","SBP","SEN","SES","SNT","SPA","STA","STI","TDW",
            "TEB","TEK","TEL","TER","TES","THO","TRZ","TUR","UCK","ULP","UZB","YTO"});

            var config = AngleSharp.Configuration.Default.WithDefaultLoader();

            foreach (var value in courses_list)
            {
                string temp = value;

                var address = "http://www.sis.itu.edu.tr/tr/ders_programlari/LSprogramlar/prg.php?fb=" + temp;

                var document = await BrowsingContext.New(config).OpenAsync(address);


                var cellSelector1 = "tr td:nth-child(1)";//Crn
                var cellSelector2 = "tr td:nth-child(2)";//Course Code
                var cellSelector3 = "tr td:nth-child(3)";//Course Title
                var cellSelector4 = "tr td:nth-child(4)";//Instructor
                var cellSelector5 = "tr td:nth-child(5)";//Building
                var cellSelector6 = "tr td:nth-child(6)";//Day
                var cellSelector7 = "tr td:nth-child(7)";//Time
                var cellSelector8 = "tr td:nth-child(8)";//Room
                var cellSelector9 = "tr td:nth-child(9)";//Capacity
                var cellSelector10 = "tr td:nth-child(12)";//Major Restriction

                var crn = document.QuerySelectorAll(cellSelector1).Skip(5).ToList();
                var CourseCode = document.QuerySelectorAll(cellSelector2).Skip(4).ToList();
                var CourseTitle = document.QuerySelectorAll(cellSelector3).Skip(3).ToList();
                var Instructor = document.QuerySelectorAll(cellSelector4).Skip(2).ToList();
                var Building = document.QuerySelectorAll(cellSelector5).Skip(2).ToList();
                var Day = document.QuerySelectorAll(cellSelector6).Skip(2).ToList();
                var Time = document.QuerySelectorAll(cellSelector7).Skip(2).ToList();
                var Room = document.QuerySelectorAll(cellSelector8).Skip(2).ToList();
                var Capacity = document.QuerySelectorAll(cellSelector9).Skip(2).ToList();
                var Restriction = document.QuerySelectorAll(cellSelector10).Skip(2).ToList();

                conn.Open();

                SqlCommand insertCommand = new SqlCommand();
                insertCommand = conn.CreateCommand();

                for (int i = 0; i < crn.Count - 1; i++)
                {
                    insertCommand.Parameters.Clear();

                    insertCommand.CommandText = "INSERT INTO Course_info (CRN, CourseCode,CourseTitle, Instructor," +
"Building,Day,Time,Room,Capacity,MajorRestriction)VALUES (@0, @1, @2, @3,@4,@5,@6,@7,@8,@9)";

                    insertCommand.Parameters.AddWithValue("0", crn[i].TextContent);
                    insertCommand.Parameters.AddWithValue("1", CourseCode[i].TextContent);
                    insertCommand.Parameters.AddWithValue("2", CourseTitle[i].TextContent);
                    insertCommand.Parameters.AddWithValue("3", Instructor[i].TextContent);
                    insertCommand.Parameters.AddWithValue("4", Building[i].TextContent);
                    insertCommand.Parameters.AddWithValue("5", Day[i].TextContent);
                    insertCommand.Parameters.AddWithValue("6", Time[i].TextContent);
                    insertCommand.Parameters.AddWithValue("7", Room[i].TextContent);
                    insertCommand.Parameters.AddWithValue("8", Capacity[i].TextContent);
                    insertCommand.Parameters.AddWithValue("9", Restriction[i].TextContent);

                    insertCommand.ExecuteNonQuery();

                }
                conn.Close();
            }



        }


        public async void Check_Exam()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);


            List<string> courses_list = new List<string>(new string[]
            {"AKM","ATA","ALM","BEB","BED","BEN","BIL","BIO","BLG","BLS","BUS","CAB","CEV","CHE","CHZ","CIE","CMP",
            "COM","DEN","DFH", "DGH","DNK","DUI","EAS","ECO","ECM","EHA","EHB","EHN","EKO","ELE","ELH","ELK","ELT",
            "END","ENE","ENG","ENR","ESL","ESM","ETK","EUT","FIZ","FRA","FZK","GED","GEM","GEO","GID","GLY","GMI",
            "GMK","GSB","GSL","GUV","GVT","HUK","HSS","ICM","ILT", "IML","ING","IMS","ISE", "ISH","ISL","ISP","ITA",
            "ITB","JDF","JEF", "JEO","JPN","KIM","KMM","KMP","KON","LAT","MAD", "MAK","MAL","MAT","MEK","MEN","MET",
            "MCH","MIM","MKN","MST","MTM","MOD","MRE","MRT","MTH","MTK","MTO","MTR","MUH","MUK","MUT","MUZ","NAE",
            "NTH","PAZ","PEM","PME","PET","PHE","PHY","RES","RUS","SBP","SEN","SES","SNT","SPA","STA","STI","TDW",
            "TEB","TEK","TEL","TER","TES","THO","TRZ","TUR","UCK","ULP","UZB","YTO"});

            var config = AngleSharp.Configuration.Default.WithDefaultLoader();

            //foreach (var value in courses_list)
            //{
            //    string temp = value;

            //    var address = "http://www.sis.itu.edu.tr/tr/sinav_programi/sinavlist.php";

            //    var document = await BrowsingContext.New(config).OpenAsync(address);

            //    var doc = await GetActive().QuerySelector<IHtmlFormElement>("CAB");



            //}






        }

    }
}