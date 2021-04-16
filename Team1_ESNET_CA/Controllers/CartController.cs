﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Team1_ESNET_CA.Data;
using Team1_ESNET_CA.Models;

namespace Team1_ESNET_CA.Controllers
{
   
    public class CartController : Controller
    {
        protected static readonly string connectionString = "Server=(local);Database=Necrosoft_14_04_21; Integrated Security=true";
        
       

        private readonly AppData appData;

        public CartController(AppData appData)
        {
            this.appData = appData;
        }
  
        public IActionResult AddToCart( string Email , Product pdt,Cart c)
        {
            
           
           
       
            c.Total_Qty_Cart = c.Total_Qty_Cart + c.Quantity;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"insert into Cart (Cart_ID,Email,Total_Qty_Cart)
                                Values(@Cart_ID,@Email,@Total_Qty_Cart)";

                string sql1 = @"insert into Cart_Product(Cart_ID,Product_ID,Quantity)
                                Values(@Cart_ID,@Product_ID,@Quantity)";

                string sql2 = @"select Session_ID,Email from Session";
                
                SqlCommand cmdEmail = new SqlCommand(sql, conn);
                SqlCommand cmdNoEmail = new SqlCommand(sql, conn);
                SqlCommand cmd = new SqlCommand(sql1, conn);
                SqlCommand cmd1 = new SqlCommand(sql2, conn);


             
                string sessionId = Request.Cookies["sessionId"];

                if (sessionId != null)
                {
                    Session session = appData.Sessions.FirstOrDefault(x =>x.Email == Email);
                    if (session != null)
                    {
                        c.Cart_ID = sessionId;
                        c.Email = Email;
                        cmdEmail.Parameters.AddWithValue("@Cart_ID", c.Cart_ID);
                        cmdEmail.Parameters.AddWithValue("@Email", c.Email);
                        cmdEmail.Parameters.AddWithValue("@Total_Qty_Cart", c.Total_Qty_Cart);
                        cmdEmail.ExecuteNonQuery();

                        cmd.Parameters.AddWithValue("@Cart_ID", c.Cart_ID);
                        cmd.Parameters.AddWithValue("@Product_ID", pdt.Product_ID);
                        cmd.Parameters.AddWithValue("@Quantity", c.Quantity);
                        cmd.ExecuteNonQuery();
                    }

                    else
                    {
                        if(c.Total_Qty_Cart==0)
                        {

                        }
                        c.Cart_ID = sessionId;
                        cmdNoEmail.Parameters.AddWithValue("@Cart_ID", c.Cart_ID);
                        cmdNoEmail.Parameters.AddWithValue("@Email", "NULL");
                        cmdNoEmail.Parameters.AddWithValue("@Total_Qty_Cart", c.Total_Qty_Cart);

                        cmdNoEmail.ExecuteNonQuery();

                        cmd.Parameters.AddWithValue("@Cart_ID", c.Cart_ID);
                        cmd.Parameters.AddWithValue("@Product_ID", pdt.Product_ID);
                        cmd.Parameters.AddWithValue("@Quantity", c.Quantity);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    c.Cart_ID = Guid.NewGuid().ToString();
                    cmdNoEmail.Parameters.AddWithValue("@Cart_ID", c.Cart_ID);
                    cmdNoEmail.Parameters.AddWithValue("@Email", "NULL");
                    cmdNoEmail.Parameters.AddWithValue("@Total_Qty_Cart", c.Total_Qty_Cart);
                    cmdNoEmail.ExecuteNonQuery();
                    
                    cmd1.Parameters.AddWithValue("@Cart_ID", c.Cart_ID);
                   cmd1.Parameters.AddWithValue("@Product_ID", pdt.Product_ID);
                    cmd1.Parameters.AddWithValue("@Quantity", c.Quantity);
                   
                    cmd1.ExecuteNonQuery();
                }
                
                ViewData["Total_Qty_Cart"] = c.Total_Qty_Cart;

                return RedirectToAction("Index", "Gallery");
            }

        }
    }
}

