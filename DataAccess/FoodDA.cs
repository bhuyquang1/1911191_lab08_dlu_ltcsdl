using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class FoodDA
    {
        public List<Food> GetAll()
        {
            //Khai báo đối tượng SqlConnection và mở kết nối 
            //Đối tượng SqlConnection truyền vào chuỗi kết nối trong App.config 
            SqlConnection sqlConn = new SqlConnection(Ultilities.ConnectionString);
            sqlConn.Open();
            //Khai báo đối tượng SqlCommand có kiểu xử lý là StoredProcedure 
            SqlCommand command = sqlConn.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = Ultilities.Food_GetAll;
            // Đọc dữ liệu, trả về danh sách các đối tượng Food 
            SqlDataReader reader = command.ExecuteReader();
            List<Food> list = new List<Food>();
            while (reader.Read())
            {
                Food food = new Food();
                food.ID = Convert.ToInt32(reader["ID"]);
                food.Name = reader["Name"].ToString();
                food.Unit = reader["Unit"].ToString();
                food.FoodCategoryID = Convert.ToInt32(reader["FoodCategoryID"]);
                food.Price = Convert.ToInt32(reader["Price"]);
                food.Notes = reader["Notes"].ToString();
                list.Add(food);
            }
            sqlConn.Close();
            return list;
        }
        public int Insert_Update_Delete(Food food, int action)
        {
            // Khai báo đối tượng SqlConnection và mở kết nối  
            // Đối tượng SqlConnection truyền vào chuỗi kết nối trong App.config 
            SqlConnection sqlConn = new SqlConnection(Ultilities.ConnectionString);
            sqlConn.Open();
            //Khai báo đối tượng SqlCommand có kiểu xử lý là StoredProcedure 
            SqlCommand command = sqlConn.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = Ultilities.Food_InsertUpdateDelete;
            SqlParameter IDPara = new SqlParameter("@ID", SqlDbType.Int);
            IDPara.Direction = ParameterDirection.InputOutput;
            command.Parameters.Add(IDPara).Value = food.ID;
            //Các biến còn lại chỉ truyền vào       
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 1000)
                                    .Value = food.Name;
            command.Parameters.Add("@Unit", SqlDbType.NVarChar)
                              .Value = food.Unit;
            command.Parameters.Add("@FoodCategoryID", SqlDbType.Int)
                              .Value = food.FoodCategoryID;
            command.Parameters.Add("@Price", SqlDbType.Int)
                              .Value = food.Price;
            command.Parameters.Add("@Notes", SqlDbType.NVarChar, 3000)
                              .Value = food.Notes;
            command.Parameters.Add("@Action", SqlDbType.Int)
                              .Value = action;
            int result = command.ExecuteNonQuery();
            // Thực thi lệnh  
            if (result > 0) // Nếu thành công thì trả về ID đã thêm 
                return (int)command.Parameters["@ID"].Value;
            return 0;
        }
    }
}
