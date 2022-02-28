using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.HtmlControls;

namespace GoWebForms
{
    public partial class variables : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Web.UI.HtmlControls.HtmlButton btn;

            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("ingresar.aspx", true);
            }
            else
            {
                BtnUser.InnerHtml = "<i class='fas fa-user'></i> " + User.Identity.Name;

                if (Session["UserRole"] == null)
                    Session["UserRole"] = GetUserRole(User.Identity.Name);

                btn = (System.Web.UI.HtmlControls.HtmlButton)this.FindControl("BtnUser");
                btn.Attributes["title"] = Session["UserRole"].ToString();

                
            }
        }

   

        public string GetUserRole(string UserName)
        {
            SqlCommand cmd = new SqlCommand();
            string UserRole = "";

            try
            {
                string cs = ConfigurationManager.ConnectionStrings["AlmacenesConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(cs);

                cmd.Connection = conn;

                cmd.CommandText = "[secure].[sp_get_UserRole]";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserName", UserName);



                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserRole = dr["Role"].ToString();
                    }
                }

                conn.Close();


                return UserRole;

            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
                return null;
            }
        }

        protected void FormView1_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                ErrorLabel.Text = "Error : " + e.Exception.Message;

                e.ExceptionHandled = true;
            }
            else
            {
                ErrorLabel.Text = "Inserted Successfully";
            }

            Response.Redirect("variables.aspx");
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("variables.aspx");
        }
        protected void EditFormView_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            EditFormView.ChangeMode(e.NewMode);
        }

        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("ingresar.aspx", true);
        }

       

        protected void GetRecordToUpdate(String ID)
        {

            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(VariableDS.ConnectionString);

            cmd = new SqlCommand("go.[sp_Variables_get_Variable]", con);
            cmd.Parameters.Add(new SqlParameter("@cod_variable", ID));
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adp = new SqlDataAdapter();

            con.Open();

            adp.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adp.Fill(ds);

            EditFormView.DataSource = ds;
            EditFormView.DataBind();

            con.Close();
        }


        protected void DeleteRecord(String ID)
        {

            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(VariableDS.ConnectionString);

            cmd = new SqlCommand("go.[sp_Variables_delete]", con);
            cmd.Parameters.Add(new SqlParameter("@cod_variable", ID));



            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            cmd.ExecuteNonQuery();



            con.Close();
        }

        protected void EditFormView_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            DataKey key = EditFormView.DataKey;



            try
            {
                //Obtengo los valores de los campos a editar
                TextBox txtcod_variable = (TextBox)EditFormView.FindControl("txtcod_variable");
                TextBox txtdescripcion = (TextBox)EditFormView.FindControl("txtdescripcion");
                TextBox txtabreviado = (TextBox)EditFormView.FindControl("txtabreviado");
                TextBox txtselect_campo = (TextBox)EditFormView.FindControl("txtselect_campo");
                TextBox txtfrom_tabla = (TextBox)EditFormView.FindControl("txtfrom_tabla");
                TextBox txtwhere_condicion = (TextBox)EditFormView.FindControl("txtwhere_condicion");
                TextBox txtcampo_funcionario = (TextBox)EditFormView.FindControl("txtcampo_funcionario");


                //DateTime isoDateTime = DateTime.ParseExact(txtCalendar.Value, format, CultureInfo.InvariantCulture);

                SqlConnection conn = new SqlConnection(VariableDS.ConnectionString);

                cmd.Connection = conn;

                cmd.CommandText = "go.sp_Variables_update";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@cod_variable", txtcod_variable.Text);
                cmd.Parameters.AddWithValue("@descripcion", txtdescripcion.Text);
                cmd.Parameters.AddWithValue("@abreviado", txtabreviado.Text);
                cmd.Parameters.AddWithValue("@select_campo", txtselect_campo.Text);
                cmd.Parameters.AddWithValue("@from_tabla", txtfrom_tabla.Text);
                cmd.Parameters.AddWithValue("@where_condicion", txtwhere_condicion.Text);
                cmd.Parameters.AddWithValue("@campo_funcionario", txtcampo_funcionario.Text);



                conn.Open();
                cmd.ExecuteNonQuery();

                conn.Close();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "",
                "$('#editModal').modal('hide');", true);

                Response.Redirect("Variables.aspx");


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        protected void EditFormView_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            EditFormView.ChangeMode(FormViewMode.ReadOnly);

            VariableListView.DataBind();

        }
        protected void ListView_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                GetRecordToUpdate(e.CommandArgument.ToString());

                ScriptManager.RegisterStartupScript(this, this.GetType(), "",
               "$('#editModal').modal('show');", true);



            }
            else if (e.CommandName == "Eliminar")
            {
                DeleteRecord(e.CommandArgument.ToString());
                VariableListView.DataBind();
            }
          
        }
    }
}