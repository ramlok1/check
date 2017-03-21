using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
using System.Data;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Net;
using System.Web.Services;

namespace AppCIELO
{
    enum NotificationType
    {
        info,
        ok,
        error
    }

    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }
    public partial class WebForm1 : System.Web.UI.Page
    {
        private string reserva;
        private string habitacion;
        private UConnection DB;
        private DateTime llegada;
        private DateTime salida;
        private string cajaSeg;
        private string agencia;
        private string mayorista;
        private string apellido;
        private string nombre;
        private string paseSalida;
        private string pax;        
        private string mensaje;
        private bool lang = true;
        ArrayList arrbr;
        BrazaR br;
        DateTime fecCont;
        string fecha_actual;
        string fecha_cont;
        private string hostname;
        private string ipName;
        int conteo;


        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                
                ViewState["idioma"] = lang;
                ViewState["texto"] = "Please enter your bracelet number:";
                lblBand.Text = "Please enter your bracelet number:";
                lblTime.Text = DateTime.Now.ToString("ddd d MMMM hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"));
                DB = new UConnection("192.168.1.8", "cronos", "frgrand", "service");
            }
            else
            {
                reserva = (string)ViewState["reserva"];
                habitacion = (string)ViewState["habitacion"];
                apellido = (string)ViewState["apellido"];
                nombre = (string)ViewState["nombre"];
                if (ViewState["fecont"] != null) { fecCont = (DateTime)ViewState["fecont"]; }

                lang = (bool)ViewState["idioma"];
                lblBand.Text = (string)ViewState["texto"]; 
                lblTime.Text = DateTime.Now.ToString("ddd d MMMM hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"));
                DB = new UConnection("192.168.1.8", "cronos", "frgrand", "service");
            }
            //Verificar imrpesora
            verifica_impresora();
     
        }
        private void verifica_impresora()
        {
            hostname = (Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName);
            hostname = hostname.ToUpper();

            if (hostname.Contains(".GOC"))
            {
                hostname = hostname.Replace(".GOC", "");
            }

            ipName = obtieneNombreIP(hostname);
            if (String.IsNullOrEmpty(ipName))
            {
                if (lang) { lblaviso.Text = "Printer error, please got to the front desk"; }
                else { lblaviso.Text = "Error en impresora favor de pasar a recepción"; }
                btnEnviar.Visible = false;
                btnSalida.Visible = false;
            }
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            
            
            lblaviso.Text = "";
            if (txtBrazalete.Text == "")
            {
                //Mensaje segun idioma
                if (lang) { ShowNotification("Bracelet empty", "Please, you must enter the bracelet number to cheack out.", NotificationType.info); }
                else { ShowNotification("Brazalete vacio", "Debe ingresar su numero de brazalete para poder realizar el check out.", NotificationType.info); }
                Timer1.Enabled = true;
                return;
            }
            if (txthabi.Text == "")
            {
                //Mensaje segun idioma
                if (lang) { ShowNotification("Room empty", "Please, you must enter the room number to cheack out.", NotificationType.info); }
                else { ShowNotification("Habitacion vacio", "Debe ingresar su numero de habitación para poder realizar el check out.", NotificationType.info); }
                Timer1.Enabled = true;
                return;
            }
            if (txthabi.Text.Length>6)
            {
                //Mensaje segun idioma
                if (lang) { ShowNotification("Too many chars in room field", "Please, verify.", NotificationType.info); }
                else { ShowNotification("Habitacion no existe", "Favor de verificar.", NotificationType.info); }
                Timer1.Enabled = true;
                return;
            }
            // reviso fechas de contabilidad, check out y actual
            // obtengo la fecha de contabilidad
            fecCont = obtenerFechaContabilidad();
            DateTime Hoy = DateTime.Today;
            fecha_actual = Hoy.ToString("dd-MM-yyyy");
            fecha_cont = fecCont.ToString("dd-MM-yyyy");
            // si es diferente la fecha de hoy contra la de la contabilidad, no pasa
            if (fecha_actual != fecha_cont)
            {
                if (lang) { ShowNotification("System problem", "Please go to the front desk.", NotificationType.error); }                
                else { ShowNotification("Error en sistema", "Favor de ir a recepción.", NotificationType.error); }
                DB.Dispose();
                Timer1.Enabled = true;
                return;
            }

            // obtengo el numero de reservacion por medio del brazalete
            obtenerReservaPorBrazalete();
            int cbr = arrbr.Count;
            if (cbr==0)
            {
                if (lang) { ShowNotification("Bracelet not found", "Please verify the bracelet number and try again.", NotificationType.error); }
                else {ShowNotification("Bracelete no encontrado","Por favor verifique el numero de brazalete e intentelo de nuevo.", NotificationType.error);   }               
                DB.Dispose();
                Timer1.Enabled = true;
            }
            else if (cbr==1)
            {
           
                reserva = ((BrazaR)arrbr[0]).Reserva;
                ViewState["reserva"] = reserva;
                validacionesCheckout();
                
            }
            else
            {
                
                if (lang) { ShowNotification("Select your reservation", "Please, select you reservation based on the register name and room .", NotificationType.info); }
                else { ShowNotification("Seleccione su reservación", "Por favor seleccione su reserva basandose en el nombre registrado y la habitación.", NotificationType.info); }
                ArraytoGrid();
                mpePopUp.Show();
                
            }
        }
        private void ArraytoGrid()
        {
              
        DataTable dt = new DataTable();
            dt.Columns.Add("Reserva");
            dt.Columns.Add("Room");
            dt.Columns.Add("#Bracelet");
            dt.Columns.Add("Name");

            for (int i=0;i<arrbr.Count;i++)
            {
                dt.Rows.Add();
                dt.Rows[i]["Reserva"] = ((BrazaR)arrbr[i]).Reserva;
                dt.Rows[i]["Room"] = ((BrazaR)arrbr[i]).Habi;
                dt.Rows[i]["#Bracelet"] = ((BrazaR)arrbr[i]).Folio;
                dt.Rows[i]["Name"] = ((BrazaR)arrbr[i]).Nombre;

            }
            Arrgrid.DataSource = dt;
            Arrgrid.DataBind();
        }

        private void validacionesCheckout()
        {
            // obtengo los datos de la reserva
            obtenerDatosReserva();
            // OBTENGO LOS DATOS DEL HUESPED Y LOS VEO EN EL GRID
            obtenerDatosHuesped();
            obtenerDatosHuesped2();
            // reviso si hay cheques abiertos
            int cheques = obtenerChequesAbiertos(fecCont);
            //Revisa salida anticipada

            if (salida.ToString("dd-MM-yyyy") != fecha_cont)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "conteo", "count('" + 10 + "');", true);
                if (lang) { ShowNotification("Anticipated check out", "For anticipated check out, please go to the front desk.", NotificationType.error); }                
                else { ShowNotification("Salida anticipada", "Para realizar salida anticipada, favor de pasar a la recepción.", NotificationType.error); }
                mensaje = "SALIDA ANTICIPADA";                
                inserta_Modifi(fecCont, "Salida Anticipada");
                imprimePase("000", false, hostname, ipName);
                DB.Dispose();
                //Timer1.Enabled = true;
                return;
            }

            if (cheques > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "conteo", "count('" + 10 + "');", true);
                if (lang) { ShowNotification("Account issue", "Room with problems in the account, please go to the front desk.", NotificationType.error); }                
                else { ShowNotification("Problema en la cuenta", "Habitacion con problemas en la cuenta, favor de pasar a recepción.", NotificationType.error); }                
                imprimePase("000", false, hostname, ipName);
                mensaje = "CHEQUES ABIERTOS";
                inserta_Modifi(fecCont, "Cheques Abiertos");
                DB.Dispose();
               // Timer1.Enabled = true;
            }
            else
            {
                // reviso si tiene pendientes
                int observaciones = obtenerListaPendientes();

                if (observaciones > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "conteo", "count('" + 10 + "');", true);
                   if (lang) { ShowNotification("Non-attended requirements", "Room with non-attended requirements, please go to front desk.", NotificationType.error); }                    
                    else { ShowNotification("Requerimientos no atendidos", "Habitacion con requrimientos no atendidos, favor de pasar a recepción.", NotificationType.error); }
                    mensaje = "REQUERIMIENTOS SIN ATENDER";
                    imprimePase("000", false, hostname, ipName);
                    inserta_Modifi(fecCont, "Requerimientos sin atender");
                    DB.Dispose();
                    
                   //Timer1.Enabled = true;
                }
                else
                {
                    // reviso si ya entrego el candado de la caja de seguridad
                    if (!String.IsNullOrEmpty(cajaSeg))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "conteo", "count('" + 10 + "');", true);
                        if (lang) { ShowNotification("Safe lock", "Room with safe lock, please go to front desk.", NotificationType.error); }                        
                        else { ShowNotification("Caja de seguridad", "Habitacion con candado, por favor ir a recepción a realizar la entrega.", NotificationType.error); }
                        mensaje = "CAJA DE SEGURIDAD";
                        imprimePase("000", false, hostname, ipName);
                        inserta_Modifi(fecCont, "Falta Candado");
                        DB.Dispose();
                        //Timer1.Enabled = true;
                    }
                    else
                    {
                        // reviso el balance de los movimientos para saber si esta en ceros
                        double edocta = obtenerBalanceMovimientos();

                        if (edocta != 0.0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "conteo", "count('" + 10 + "');", true);
                            if (lang) { ShowNotification("Balance to be covered", "Room with balance to be covered, please go to front desk", NotificationType.error); }                            
                            else { ShowNotification("Saldo por cubrir", "Habitacion con saldo por cubir, favor de pasar a recepción.", NotificationType.error); }
                            mensaje = "SALDO EN CUENTA";
                            imprimePase("000", false, hostname, ipName);
                            inserta_Modifi(fecCont, "Saldo en cuenta");
                            DB.Dispose();
                            //Timer1.Enabled = true;
                        }
                        else
                        {
                            btnEnviar.Visible = false;
                            btnSalida.Visible = true;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "conteo", "count('" + 30 + "');", true);



                        } // cuenta en ceros
                    } // candado
                } // existen observaciones
            } // toallas pendientes
        }
        private void obtenerReservaPorBrazalete()
        {
             arrbr = new ArrayList();
             

            
            string sql = "select BR_PREFIJO, BU_FOLIO,RV_RESERVA,RV_HABI,VN_APELLIDO||' '||VN_NOMBRE AS NOMBRE" +
                    " from FRBRAZACOLORES" +
                    " INNER JOIN FRBRAZALUSO ON BR_COLOR=BU_COLOR" +
                    " INNER JOIN FRESERVA ON BU_RESERVA=RV_RESERVA" +
                    " INNER JOIN FRESERNO ON RV_RESERVA=VN_RESERVA" +
                   // " WHERE RV_STATUS='B' "+
                    " WHERE RV_STATUS='B' AND RV_SALIDA='" + fecCont.ToString("dd/MMM/yy", CultureInfo.CreateSpecificCulture("en-US")) + "'" +
                    " AND VN_SECUENCIA=1 AND BU_FOLIO LIKE '%" + txtBrazalete.Text + "%' AND RV_HABI='"+txthabi.Text+"'";
            

            try
            {
                if (DB.EjecutaSQL(sql))
                {
                    
                    while (DB.ora_DataReader.Read())
                    {
                        br = new BrazaR();
                        //reserva = Convert.ToString(DB.ora_DataReader["BU_RESERVA"]);
                        br.Prefijo = Convert.ToString(DB.ora_DataReader["BR_PREFIJO"]);
                        br.Folio = Convert.ToString(DB.ora_DataReader["BU_FOLIO"]);
                        br.Reserva = Convert.ToString(DB.ora_DataReader["RV_RESERVA"]);
                        br.Habi = Convert.ToString(DB.ora_DataReader["RV_HABI"]);
                        br.Nombre = Convert.ToString(DB.ora_DataReader["NOMBRE"]);

                        arrbr.Add(br);

                    }
                }
            }
            catch (Exception ex)
            {
                lblaviso.Text = "OCURRIÓ UN ERROR AL TRATAR DE CONECTARSE A LA BASE DE DATOS";
            }
        }

        private void obtenerDatosReserva()
        {
            string adulto = "";
            string menor = "";
            string sql4 = string.Format("select * from freserva where rv_reserva = '{0}'", reserva);
            
            if (DB.EjecutaSQL(sql4))
            {
                while (DB.ora_DataReader.Read())
                {
                    llegada = Convert.ToDateTime(DB.ora_DataReader["RV_LLEGADA"]);
                    salida = Convert.ToDateTime(DB.ora_DataReader["RV_SALIDA"]);
                    cajaSeg = Convert.ToString(DB.ora_DataReader["RV_CAJA_SEG"]);
                    habitacion = Convert.ToString(DB.ora_DataReader["RV_HABI"]);
                    agencia = Convert.ToString(DB.ora_DataReader["RV_AGENCIA"]);
                    mayorista = Convert.ToString(DB.ora_DataReader["RV_MAYORISTA"]);
                    adulto = Convert.ToString(DB.ora_DataReader["RV_ADULTO"]);
                    menor = Convert.ToString(DB.ora_DataReader["RV_MENOR"]);
                }
                ViewState["habitacion"] = habitacion;
            }
            pax = adulto + "." + menor;
        }

        private void obtenerDatosHuesped()
        {
            var dt = new DataTable();
            string sql = string.Format("select vn_reserva, vn_apellido, vn_nombre from freserno where vn_reserva = '{0}' and vn_secuencia = 1", reserva);
            if (DB.EjecutaSQL(sql))
            {
                if (DB.ora_DataReader.HasRows)
                {
                    dt.Load(DB.ora_DataReader);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }

        private void obtenerDatosHuesped2()
        {
            var dt = new DataTable();
            string sql = string.Format("select vn_reserva, vn_apellido, vn_nombre from freserno where vn_reserva = '{0}' and vn_secuencia = 1", reserva);
            if (DB.EjecutaSQL(sql))
            {
                if (DB.ora_DataReader.HasRows)
                {
                    while (DB.ora_DataReader.Read())
                    {
                        apellido = Convert.ToString(DB.ora_DataReader["vn_apellido"]);
                        nombre = Convert.ToString(DB.ora_DataReader["vn_nombre"]);
                    }
                    ViewState["apellido"] = apellido;
                    ViewState["nombre"] = nombre;
                }
            }
        }

        private int obtenerChequesAbiertos(DateTime fechaCont)
        {
            int cantCheques = 0;
            string fecha_actual = fechaCont.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            // reviso puntos de venta
            string sql3 = string.Format("select count(*) cantidad from pvcheqdiaenc, FRMOVCO where CE_RESERVA = '{0}' " +
                                         "and CE_FECHA = '{1}' and CE_CIERRA_U is null and CO_MOVI = CE_MOVI and CO_FASE = CE_FASE", reserva, fecha_actual);
            if (DB.EjecutaSQL(sql3))
            {
                while (DB.ora_DataReader.Read())
                {
                    cantCheques = Convert.ToInt16(DB.ora_DataReader["cantidad"]);
                }
            }
            return cantCheques;
        }

        private DateTime obtenerFechaContabilidad()
        {
            DateTime Hoy = DateTime.Today;
            string sql = string.Format("SELECT PR_FECHA FROM FRPARAM");
            if (DB.EjecutaSQL(sql))
            {
                while (DB.ora_DataReader.Read())
                {
                    Hoy = Convert.ToDateTime(DB.ora_DataReader["PR_FECHA"]);
                }
                ViewState["fecont"] = Hoy;
            }
            return Hoy;
        }

        private int obtenerListaPendientes()
        {
            int obsv = 0;
            string fecha_llegada = llegada.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            string fecha_salida = salida.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            string sql5 = string.Format("SELECT COUNT(*) OBS FROM FROBSRVA WHERE OR_RESERVA='{0}' AND OR_COMPLETADO=0 AND OR_FECHA BETWEEN '{1}' AND '{2}'",
                                    reserva, fecha_llegada, fecha_salida);
            if (DB.EjecutaSQL(sql5))
            {
                while (DB.ora_DataReader.Read())
                {
                    obsv = Convert.ToInt16(DB.ora_DataReader["OBS"]);
                }
            }
            return obsv;
        }

        private double obtenerBalanceMovimientos()
        {
            double balance = 0.0;
            string result = "";
            string sql6 = string.Format("select sum(nvl(mo_importe,0)-nvl(mo_descontado,0)) BALANCE from frmovim, frmovco where mo_referencia = '{0}' " +
                    "and mo_movi = co_movi and mo_fase = co_fase and co_no_cobro = 'N'", reserva);
            if (DB.EjecutaSQL(sql6))
            {
                while (DB.ora_DataReader.Read())
                {
                    result = Convert.ToString(DB.ora_DataReader["BALANCE"]);
                }
            }

            if (String.IsNullOrEmpty(result))
            {
                balance = 0.0;
            }
            else
            {
                balance = Convert.ToDouble(result);
            }
            return balance;
        }

        private bool doCheckout(DateTime fechaCont)
        {
            string sp = "do_Checkout";
            string fecha_actual = fechaCont.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            bool ok;
            OracleCommand cmd = new OracleCommand();
            cmd.Parameters.Add(new OracleParameter("@Reserva", reserva));
            cmd.Parameters.Add(new OracleParameter("@Habi", habitacion));
            cmd.Parameters.Add(new OracleParameter("@Usuario", "EXPRESS"));
            cmd.Parameters.Add(new OracleParameter("@Fecha", fecha_actual));
            cmd.Parameters.Add(new OracleParameter("@RFC", null));
            cmd.Parameters.Add(new OracleParameter("@FACTURA", null));
            ok = DB.EjecutaSP(ref cmd, sp);
            return ok;
        }

        private void inserta_Modifi(DateTime fechaCont,String motivo)
        {
            string sp = "inserta_fmodifi";
            string fecha_actual = fechaCont.ToString("dd/MMM/yy", CultureInfo.CreateSpecificCulture("en-US"));
            bool ok;
            OracleCommand cmd = new OracleCommand();
            cmd.Parameters.Add(new OracleParameter("@RESERVA", reserva));
            cmd.Parameters.Add(new OracleParameter("@FECHA", fecha_actual));
            cmd.Parameters.Add(new OracleParameter("@USUARIO", "EXPRESS"));
            cmd.Parameters.Add(new OracleParameter("@TIPO", "CHECK_OUT EXPRESS"));
            cmd.Parameters.Add(new OracleParameter("@CAMPO", "EXPRESS"));
            cmd.Parameters.Add(new OracleParameter("@ANTES", null));
            cmd.Parameters.Add(new OracleParameter("@DESPUES", lblaviso.Text));
            cmd.Parameters.Add(new OracleParameter("@MOTIVO", motivo));
            ok = DB.EjecutaSP(ref cmd, sp);            
        }

        private bool docoPases()
        {
            bool ok;
            string sp = "do_coPases";
            OracleCommand cmd = new OracleCommand();
            cmd.Parameters.Add(new OracleParameter("@Reserva", reserva));
            cmd.Parameters.Add(new OracleParameter("@Mayo", mayorista));
            cmd.Parameters.Add(new OracleParameter("@Agen", agencia));
            ok = DB.EjecutaSP(ref cmd, sp);
            return ok;
        }

        private string obtenerFolioPaseSalida()
        {
            string folio = "";
            string sql = "select lpad(to_char(PASE_SALIDA_ID.NextVal),6,'0') FOLIO from DUAL";
            if (DB.EjecutaSQL(sql))
            {
                while (DB.ora_DataReader.Read())
                {
                    folio = Convert.ToString(DB.ora_DataReader["FOLIO"]);
                }
            }
            return folio;
        }

        private void imprimePase(string folio, bool check, string hostname, string ipName)
        {
            DateTime Hoy = DateTime.Today;
            string fecha_actual = Hoy.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            string hora_actual = DateTime.Now.ToString("hh:mm");
            StringBuilder textoAgua = new StringBuilder();
            textoAgua.Append("                          GRAND OASIS CANCUN");
            textoAgua.Append("\n\n\n                                                              Folio: " + folio);
            if (check)
            {
                textoAgua.Append("\n\n            PASE SALIDA TOTAL/CHECK-OUT TICKET");
            }else
            {
                if (lang)
                {
                    textoAgua.Append("\n\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", "THIS IS NOT A CHECK-OUT TICKET")));
                    textoAgua.Append("\n\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", "PLEASE GO TO THE FRONT DESK")));                   
                }
                else
                {
                    textoAgua.Append("\n\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", "NO ES UN PASE DE SALIDA")));
                    textoAgua.Append("\n\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", "FAVOR DE IR A RECEPCIÓN")));
                }
            }
            textoAgua.Append("\n\n\n\n Fecha: " + fecha_actual + "            Hora: " + hora_actual);
            textoAgua.Append("\n\n Habitacion:    " + habitacion);
            textoAgua.Append("\n\n Reservacion:   " + reserva);
            textoAgua.Append("\n\n Numero de Pax: " + pax);
            textoAgua.Append("\n\n Cajero:         CHECK OUT EXPRESS");

            if (check)
            {
                if (lang)
                {
                    textoAgua.Append("\n\n\n\n                PLEASE GO WITH  BELL BOY");
                    textoAgua.Append("\n               TO REMOVE THE BRACELET");
                    textoAgua.Append("\n\n\n\n\n\n\n\n                 ___________________________");
                    string s = apellido + " " + nombre;
                    string line2 = String.Format("{0,-70}", String.Format("{0," + ((70 + s.Length) / 2).ToString() + "}", s));
                    textoAgua.Append("\n" + line2);
                }
                else
                {
                    textoAgua.Append("\n\n\n\n                FAVOR DE PASAR CON EL BELL BOY");
                    textoAgua.Append("\n               PARA QUE LE RETIRE EL BRAZALETE");
                    textoAgua.Append("\n\n\n\n\n\n\n\n                 ___________________________");
                    string s = apellido + " " + nombre;
                    string line2 = String.Format("{0,-70}", String.Format("{0," + ((70 + s.Length) / 2).ToString() + "}", s));
                    textoAgua.Append("\n" + line2);
                }
            }
            else
            {
                textoAgua.Append("\n\n\n\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", mensaje)));
                textoAgua.Append("\n\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", "NO VALIDO PARA SALIDA DEL HOTEL")));
                textoAgua.Append("\n" + String.Format("{0,-70}", String.Format("{0," + ((70 + mensaje.Length) / 2).ToString() + "}", " ERROR EN CHECK-OUT")));
            }
            
            textoAgua.Append("\n\n\n                   WWW.OASISHOTELES.COM");

            //MsgBox(textoAgua.ToString(), this.Page, this);
            paseSalida = textoAgua.ToString();
            // obtiene el nombre de la maquina                

            verifica_impresora();
            if (!String.IsNullOrEmpty(ipName))
            {
                //MsgBox(hostname+"--"+ipName, this.Page, this);                
                PrintDocument formulario = new PrintDocument();
                formulario.PrintPage += new PrintPageEventHandler(datosVoucher);
                formulario.PrinterSettings.PrinterName = ipName;
                string GS = Convert.ToString((char)29);
                string ESC = Convert.ToString((char)27);

                formulario.Print();
                if (check) { formulario.Print(); }
            }
      
        }

        private string obtieneNombreIP(string hostname)
        {
            string ipName = "";          
            // obtengo el nombre de la ip para la impresora con la que se va a imprimir
            UConnection DB = new UConnection("192.168.1.8", "cronos", "frgrand", "service");
            string sql = "select SU_IMPRESORA from FRSALIDA_IMPNAME where SU_NAME = '" + hostname + "' and SU_TIPO = 'PASE'";

            try
            {
                if (DB.EjecutaSQL(sql))
                {
                    while (DB.ora_DataReader.Read())
                    {
                        ipName = Convert.ToString(DB.ora_DataReader["SU_IMPRESORA"]);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("OCURRIÓ UN ERROR AL TRATAR DE OBTENER LA IMPRESORA PARA EL VOUCHER." + ex.Message, "FAVOR DE REPORTARLO A SU ÁREA DE SISTEMAS");
            }
            finally
            {
                DB.Dispose();
            }
            return ipName;
        }

        private void datosVoucher(object obj, PrintPageEventArgs ev)
        {
            Font fuente = new Font("Arial", 8);

            ev.Graphics.DrawString(paseSalida, fuente, Brushes.Black, 0, 0, new StringFormat());
        }

        public void MsgBox(String ex, Page pg, Object obj)
        {
            string s = "<SCRIPT language='javascript'>alert('" + ex.Replace("\r\n", "\\n").Replace("'", "") + "'); </SCRIPT>";
            Type cstype = obj.GetType();
            ClientScriptManager cs = pg.ClientScript;
            cs.RegisterClientScriptBlock(cstype, s, s.ToString());
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            lblaviso.Text = "";
            txtBrazalete.Text = "";
            txthabi.Text = "";
            GridView1.DataSource = null;
            GridView1.DataBind();
            Arrgrid.DataSource = null;
            Arrgrid.DataBind();
            btnEnviar.Visible = true;
            btnSalida.Visible = false;
        }

        protected void Unnamed1_Click(object sender, ImageClickEventArgs e)
        {
            lang = false;
            ViewState["idioma"] = lang;
            ViewState["texto"] = "Favor de ingresar su número de brazalete:";
            lblBand.Text = "Favor de ingresar su número de brazalete:";
        }

        protected void Timer_hora_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("ddd d MMMM hh:mm:ss tt",CultureInfo.CreateSpecificCulture("en-US"));
           
        }

        protected void Arrgrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            reserva = Arrgrid.SelectedRow.Cells[1].Text;
            ViewState["reserva"] = reserva;
            validacionesCheckout();
          //  btnEnviar.Visible = false;
           // btnSalida.Visible = true;

        }

        protected void Arrgrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#CCCCCC';";
                e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                e.Row.ToolTip = "Click select button for selecting this row.";
                e.Row.Cells[1].Visible = false;                
            }

        }

        private void ShowNotification(string title, string msg, NotificationType nt)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Alert", "pnotifySuccess('" + title + "','" + msg + "','" + nt.ToString() + "');", true);
        }

        protected void Unnamed2_Click(object sender, ImageClickEventArgs e)
        {
            lang = true;
            ViewState["idioma"] = lang;
            ViewState["texto"] = "Please enter your bracelet number:";
            lblBand.Text = "Please enter your bracelet number:";

        }

        protected void btnSalida_Click(object sender, EventArgs e)
        {
   
                if (doCheckout(fecCont))
                {
                    bool ok = docoPases();
                    //impresion del pase de salida
                    string folio = obtenerFolioPaseSalida();
                    imprimePase(folio, true,hostname,ipName);
                    if (!lang) { ShowNotification("Check out realizado", "Favor de entregar el pase de salida al bellboy", NotificationType.ok); }
                    else { ShowNotification("Check out done", "Please deliver the exit pass to bellboy ", NotificationType.ok); }
                    inserta_pase(folio);
                    Timer1.Enabled = true;
                }
                else
                {
                    if (lang) { ShowNotification("System problem", "Please go to the front desk.", NotificationType.error); }
                    else { ShowNotification("Error en sistema", "Favor de ir a recepción.", NotificationType.error); }
                    Timer1.Enabled = true;
                }
            

        }

 
        protected void btnlimpiar_Click(object sender, EventArgs e)
        {
            lblaviso.Text = "";
            txtBrazalete.Text = "";
            txthabi.Text = "";
            GridView1.DataSource = null;
            GridView1.DataBind();
            Arrgrid.DataSource = null;
            Arrgrid.DataBind();
            btnEnviar.Visible = true;
            btnSalida.Visible = false;

        }

        private void inserta_pase(string folio)
        {
            string fecha_actual = fecCont.ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("en-US"));
            string time = DateTime.Now.ToString("hh:mm"); 

            string sql_pase = string.Format("INSERT INTO FRSALIDA_PARCIAL "+
                "(SP_FOLIO, SP_RESERVA, SP_HABI, SP_SECUENCIA, SP_APELLIDO, SP_NOMBRE, SP_SELECCION"+
                ", SP_CAP_U, SP_CAP_F, SP_CAP_H, SP_MOTIVO, SP_OK, SP_HOTEL, SP_PENDIENTE)"+
                " VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')"
                , folio,reserva,habitacion,1,apellido,nombre,"S","EXPRESS", fecha_actual,time,"CHECK-OUT",1, "GRAND OASIS CANCUN",0);

            DB.EjecutaSQL(sql_pase);
        }
    }
}