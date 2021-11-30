﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BusinessLogic;
using DataAccess;

namespace _1911191_Lab08
{
    public partial class frmMain : Form
    {
        List<Category> listCategory = new List<Category>();
        List<Food> listFood = new List<Food>();
        Food foodCurrent = new Food();
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtPrice.Text = "0";
            txtUnit.Text = "";
            txtNotes.Text = "";
            // Thiết lập index = 0 cho ComboBox 
            if (cbCat.Items.Count > 0)
                cbCat.SelectedIndex = 0;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadCat();
            LoadFoodToLV();
        }

        private void LoadFoodToLV()
        {
            //Gọi đối tượng FoodBL từ tầng BusinessLogic  
            FoodBL foodBL = new FoodBL();
            // Lấy dữ liệu  
            listFood = foodBL.GetAll();
            int count = 1; // Biến số thứ tự 
                           // Xoá dữ liệu trong ListView  
            lvFood.Items.Clear();
            // Duyệt mảng dữ liệu để đưa vào ListView  
            foreach (var food in listFood)
            {
                // Số thứ tự  
                ListViewItem item = lvFood.Items.Add(count.ToString());
                // Đưa dữ liệu Name, Unit, price vào cột tiếp theo  
                item.SubItems.Add(food.Name);
                item.SubItems.Add(food.Unit);
                item.SubItems.Add(food.Price.ToString());
                // Theo dữ liệu của bảng Category ID, lấy Name để hiển thị 
                string foodName = listCategory.Find(x => x.ID == food.FoodCategoryID).Name;
                item.SubItems.Add(foodName);
                // Đưa dữ liệu Notes vào cột cuối 
                item.SubItems.Add(food.Notes);
                count++;
            }
        }

        private void LoadCat()
        {
            //Gọi đối tượng CategoryBL từ tầng BusinessLogic  
            CategoryBL categoryBL = new CategoryBL();
            // Lấy dữ liệu gán cho biến toàn cục listCategory 
            listCategory = categoryBL.GetAll();
            // Chuyển vào Combobox với dữ liệu là ID, hiển thị là Name 
            cbCat.DataSource = listCategory;
            cbCat.ValueMember = "ID";
            cbCat.DisplayMember = "Name";
        }

        private void lvFood_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvFood.Items.Count; i++)
            {
                // Nếu có dòng được chọn thì lấy dòng đó   
                if (lvFood.Items[i].Selected)
                {
                    foodCurrent = listFood[i];
                    txtName.Text = foodCurrent.Name;
                    txtUnit.Text = foodCurrent.Unit;
                    txtPrice.Text = foodCurrent.Price.ToString();
                    txtNotes.Text = foodCurrent.Notes;
                    // Lấy index của Combobox theo FoodCategoryID 
                    cbCat.SelectedIndex = listCategory.FindIndex(x => x.ID == foodCurrent.FoodCategoryID);
                }
            }
        }
        public int InsertFood()
        {
            //Khai báo đối tượng Food từ tầng DataAccess  
            Food food = new Food();
            food.ID = 0;
            // Kiểm tra nếu các ô nhập khác rỗng
            if (txtName.Text == "" || txtUnit.Text == "" || txtPrice.Text == "")
                MessageBox.Show("Chưa nhập dữ liệu cho các ô, vui lòng nhập lại");
            else
            {
                //Nhận giá trị Name, Unit, và Notes từ người dùng nhập vào  
                food.Name = txtName.Text;
                food.Unit = txtUnit.Text;
                food.Notes = txtNotes.Text;
                // Giá trị price là giá trị số nên cần bắt lỗi khi người dùng nhập sai  
                int price = 0;
                try
                {
                    // Cố gắng lấy giá trị  
                    price = int.Parse(txtPrice.Text);
                }
                catch
                {
                    // Nếu sai, gán giá = 0  
                    price = 0;
                }
                food.Price = price;
                // Giá trị FoodCategoryID được lấy từ ComboBox  
                food.FoodCategoryID = int.Parse(cbCat.SelectedValue.ToString());
                // Khao báo đối tượng FoodBL từ tầng Business  
                FoodBL foodBL = new FoodBL();
                // Chèn dữ liệu vào bảng  
                return foodBL.Insert(food);
            }
            return -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Gọi phương thức thêm dữ liệu 
            int result = InsertFood();
            if (result > 0) // Nếu thêm thành công  
            {
                // Thông báo kết quả  
                MessageBox.Show("Thêm dữ liệu thành công");
                // Tải lại dữ liệu cho ListView   
                LoadFoodToLV();
            }
            // Nếu thêm không thành công thì thông báo cho người dùng 
            else MessageBox.Show("Thêm dữ liệu không thành công. Vui lòng kiểm tra lại dữ liệu nhập");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xoá mẫu tin này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Khai báo đối tượng FoodBL từ BusinessLogic 
                FoodBL foodBL = new FoodBL();
                if (foodBL.Delete(foodCurrent) > 0)// Nếu xoá thành công 
                {
                    MessageBox.Show("Xoá thực phẩm thành công");
                    // Tải tữ liệu lên ListView 
                    LoadFoodToLV();
                }
                else MessageBox.Show("Xoá không thành công");
            }
        }
        public int UpdateFood() {
            Food food = foodCurrent;
            // Kiểm tra nếu các ô nhập khác rỗng  
            if (txtName.Text == "" || txtUnit.Text == "" || txtPrice.Text == "")
                MessageBox.Show("Chưa nhập dữ liệu cho các ô, vui lòng nhập lại");
            else
            {
                //Nhận giá trị Name, Unit, và Notes khi người dùng sửa  
                food.Name = txtName.Text;
                food.Unit = txtUnit.Text;
                food.Notes = txtNotes.Text;
                //Giá trị price là giá trị số nên cần bắt lỗi khi người dùng nhập sai  
                int price = 0;
                try
                {
                    // Chuyển giá trị từ kiểu văn bản qua kiểu int  
                    price = int.Parse(txtPrice.Text);
                }
                catch
                {
                    // Nếu sai, gán giá = 0  
                    price = 0;
                }
                food.Price = price;
                // Giá trị FoodCategoryID được lấy từ ComboBox  
                food.FoodCategoryID = int.Parse(cbCat.SelectedValue.ToString());
                // Khao báo đối tượng FoodBL từ tầng Business  
                FoodBL foodBL = new FoodBL();
                // Cập nhật dữ liệu trong bảng 
                return foodBL.Update(food);
            }
            return -1;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Gọi phương thức cập nhật dữ liệu 
            int result = UpdateFood();
            if (result > 0) // Nếu cập nhật thành công  
            {
                // Thông báo kết quả  
                MessageBox.Show("Cập nhật dữ liệu thành công");
                // Tải lại dữ liệu cho ListView   
                LoadFoodToLV();
            }
            // Nếu thêm không thành công thì thông báo cho người dùng 
            else MessageBox.Show("Cập nhật dữ liệu không thành công. Vui lòng kiểm tra lại dữ liệu nhập");
        }
    }
}