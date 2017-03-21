<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AppCIELO.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CHECK OUT EXPRESS</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <link rel="stylesheet" href="css/estilos.css" type="text/css" />        
    <link rel="stylesheet" href="css/loader.css" type="text/css" />    
    <link href="http://fonts.googleapis.com/css?family=Raleway" rel="stylesheet" type="text/css" />
    <script src="js/jquery-3.1.1.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="js/vkboard.js"></script> 	
	<!--[if lt IE 9]>
	<script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
	<![endif]-->		
	<script type="text/javascript" src="pnotify/pnotify.js"></script>
	<link href="pnotify/pnotify.css" rel="stylesheet" type="text/css" />
    <!-- Script de pnotify -->
    <script type="text/javascript">        
        function pnotifySuccess(t, msg, ty) {
            var clase;
            var icono;
            if (ty == "info") { clase = "infoc"; icono = "brighttheme-icon-info"; }
            if (ty == "ok") { clase = "okc"; icono = "brighttheme-icon-success"; }
            if (ty == "error") { clase = "errorc"; icono = "brighttheme-icon-error"; }
			$(function () {
				$.pnotify({
					title: t,
					text: msg,
					type: ty,					
					addclass: clase,
					history: false,					
					icon: icono,
					width: "100%",
			        nonblock: {
				    nonblock: true
				}
				});
			});
        }
        function count(t) {
            var time = t-1;
            var initialOffset = '440';
            var i = t

            /* Need initial run as interval hasn't yet occured... */


            var interval = setInterval(function () {
                $('.circle_animation').css('fill-opacity', 0.4);
                $('h2').text(i);
                if (i == 0) {                    
                    $('h2').text("");
                    document.getElementById("btnlimpiar").click();
                    clearInterval(interval);
                    return;
                }
                $('.circle_animation').css('stroke-dashoffset', initialOffset - ((i - 1) * (initialOffset / time)));

                i--;
            }, 1000);
        }
	  
	</script>
    <script><!--

    // This example shows the very basic installation
    // of the Virtual Keyboard.
    // 
    // 'keyb_change' and 'keyb_callback' functions
    // do all the job here.

    var opened = false, vkb = null, text = null;

    function keyb_change() {
        //document.getElementById("switch").innerHTML = (opened ? "Show keyboard" : "Hide keyboard");
        opened = !opened;

        if (opened && !vkb) {
            // Note: all parameters, starting with 3rd, in the following
            // expression are equal to the default parameters for the
            // VKeyboard object. The only exception is 15th parameter
            // (flash switch), which is false by default.

            vkb = new VKeyboard("keyboard",    // container's id
                                keyb_callback, // reference to the callback function
                                false,          // create the arrow keys or not? (this and the following params are optional)
                                false,          // create up and down arrow keys? 
                                false,         // reserved
                                false,          // create the numpad or not?
                                "Yu Gothic",            // font name ("" == system default)
                                "34px",        // font size in px
                                "#FFF",        // font color
                                "#F00",        // font color for the dead keys
                                "#1a1a1a",        // keyboard base background color
                                "#333333",        // keys' background color
                                "#e5e5e5",        // background color of switched/selected item
                                "#1a1a1a",        // border color
                                "#1a1a1a",        // border/font color of "inactive" key (key with no value/disabled)
                                "#1a1a1a",        // background color of "inactive" key (key with no value/disabled)
                                "#1a1a1a",        // border color of the language selector's cell
                                false,          // show key flash on click? (false by default)
                                "#CC3300",     // font color during flash
                                "#FF9966",     // key background color during flash
                                "#CC3300",     // key border color during flash
                                true,         // embed VKeyboard into the page?
                                true,          // use 1-pixel gap between the keys?
                                0);            // index(0-based) of the initial layout
        }
        else
            vkb.Show(opened);

        text = document.getElementById("txtBrazalete");
        text.focus();

        if (document.attachEvent)
            text.attachEvent("onblur", backFocus);
    }

    function backFocus() {
        if (opened) {
            var l = text.value.length;

            setRange(text, l, l);

            text.focus();
        }
    }

    // Callback function:
    function keyb_callback(ch) {
        var val = text.value;

        switch (ch) {
            case "BackSpace":
                var min = (val.charCodeAt(val.length - 1) == 10) ? 2 : 1;
                text.value = val.substr(0, val.length - min);
                break;

            case "Enter":
                keyb_change();
                break;

            default:
                text.value += ch;
        }
    }

    function setRange(ctrl, start, end) {
        if (ctrl.setSelectionRange) // Standard way (Mozilla, Opera, ...)
        {
            ctrl.setSelectionRange(start, end);
        }
        else // MS IE
        {
            var range;

            try {
                range = ctrl.createTextRange();
            }
            catch (e) {
                try {
                    range = document.body.createTextRange();
                    range.moveToElementText(ctrl);
                }
                catch (e) {
                    range = null;
                }
            }

            if (!range) return;

            range.collapse(true);
            range.moveStart("character", start);
            range.moveEnd("character", end - start);
            range.select();
        }
    }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button runat="server" ID="btnlimpiar" Text="" style="display:none;" OnClick="btnlimpiar_Click" />
        <div runat="server" class="loader loader-default is-active" id="loader"></div>
        <div id="cabecera">
            <asp:ImageButton CssClass="image" runat="server" ImageUrl="~/imagenes/mexico-flag.png" OnClick="Unnamed1_Click" />
            <asp:ImageButton CssClass="image" runat="server" ImageUrl="~/imagenes/american-flag.png" OnClick="Unnamed2_Click" />

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

             <asp:Timer ID="Timer_hora" runat="server" Enabled="true" Interval="1000" OnTick="Timer_hora_Tick"></asp:Timer>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Label CssClass="ltime" ID="lblTime" runat="server" Text="Label"></asp:Label>                   
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/imagenes/Clock.png" CssClass="imageclock" Height="30px" ImageAlign="Right" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Timer_hora" />
                </Triggers>
            </asp:UpdatePanel> 
            <div id="logo">
        

            </div>
        </div>

       <div class="item html">
        <h2></h2>
        <svg width="160" height="160" xmlns="http://www.w3.org/2000/svg">
            <g>
                <title>Layer 1</title>
                <circle id="circle" class="circle_animation" r="69.85699" cy="81" cx="81" stroke-width="8" stroke="#6fdb6f" />
            </g>
        </svg>
    </div>
        <%--    <div id="paises">
        <a href="http://twitter.com" target="_blank"><img src="imagenes/mexico-flag.png" alt="mexico" /></a>
        <a href="http://twitter.com" target="_blank"><img src="imagenes/american-flag.png" alt="american" /></a>
    </div>--%>
    
        <div class="container">
            <div>
                <h1>CHECK OUT EXPRESS</h1>
            </div>
            

            <div id="banda">      
                <asp:Label ID="lblBand" runat="server" Text=""></asp:Label>
                <%--<h2>Favor de ingresar su número de brazalete:</h2>--%>
            </div>

            <div>
                <asp:TextBox ID="txthabi" runat="server" Style="text-align: center" CssClass="txt txth" ToolTip="Please type your room" Placeholder="#Room"></asp:TextBox>
            </div>
             <div>
                <asp:TextBox ID="txtBrazalete" runat="server" Style="text-align: center" CssClass="txt" Placeholder="#Bracelet"></asp:TextBox>
            </div>

            <p>
                <a href="javascript:keyb_change()" id="switch">                  
                <img src="imagenes/teclado.gif" alt="teclado" /></a>
            </p>

            <div>
                <asp:Button ID="btnEnviar" runat="server" Text="Continue..." CssClass="btn" OnClick="btnEnviar_Click" Font-Size="X-Large" />
                 <asp:Button ID="btnSalida" runat="server" Text="DO CHECK OUT..." CssClass="btns" Visible="false" Font-Size="X-Large" OnClick="btnSalida_Click" />
            </div>  

                
     <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>
        <ajaxToolkit:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="lblHidden" PopupControlID="divPopUp" BackgroundCssClass="modalBackground"></ajaxToolkit:ModalPopupExtender>
            <!--Codigo para Modal en caso de encontrar varios brazaletes -->
            <center>
                <div id="divPopUp" class="modalpopup" style='display: none;'>                    
                    <div id="main" class="main"> 
                       <asp:GridView ID="Arrgrid" ShowHeader="False" HeaderStyle-BackColor="#333333" HeaderStyle-ForeColor="White" runat="server"  ForeColor="#3E454C"  Font-Bold="False" GridLines="None" Width="800px" Caption="Please select your reservation" CaptionAlign="Top" CellPadding="2" OnRowDataBound="Arrgrid_RowDataBound" OnSelectedIndexChanged="Arrgrid_SelectedIndexChanged"     >
                           <Columns>
                               <asp:ButtonField ButtonType="Button" CommandName="Select" Text="Select">
                               <ControlStyle CssClass="btnsg" />
                               </asp:ButtonField>
                           </Columns>
                           <EditRowStyle BackColor="#CC66FF" BorderColor="#3333CC" BorderStyle="Dotted" BorderWidth="2px" HorizontalAlign="Left" VerticalAlign="Middle" Width="100px" Wrap="True" />
                           <HeaderStyle BackColor="#666666" ForeColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Height="40px" HorizontalAlign="Center" VerticalAlign="Middle" Font-Size="Larger"></HeaderStyle>
                           <RowStyle Font-Names="Yu Gothic Light" Font-Size="X-Large" Height="80px" HorizontalAlign="Center" VerticalAlign="Middle" Width="100px" BorderColor="#663300" BorderStyle="Solid" BorderWidth="1px" />
                        </asp:GridView>
                       
                     </div>                 
                </div>
             </center>  
            <!--///////////////////////////////////////////////////////////////// -->     
            <div id="contenedor">
                <asp:GridView ID="GridView1" HeaderStyle-BackColor="#333333" HeaderStyle-ForeColor="White"
                    runat="server" AutoGenerateColumns="false" ForeColor="#3E454C" CssClass="grid" BorderStyle="None" Font-Bold="False" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="vn_reserva" HeaderText="RESERVACIÓN" ItemStyle-Width="40">
                            <ItemStyle Width="40px"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="vn_apellido" HeaderText="APELLIDO" ItemStyle-Width="150">
                            <ItemStyle Width="150px"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="vn_nombre" HeaderText="NOMBRE" ItemStyle-Width="150">
                            <ItemStyle Width="150px"></ItemStyle>
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle BackColor="#FFFFFF" ForeColor="#3E454C"></HeaderStyle>
                </asp:GridView>
            </div>
            <div id="mensaje">
                <asp:Label ID="lblaviso" runat="server" ForeColor="Red" Font-Names="Yu Gothic Medium"></asp:Label>
            </div>
        </div>
                 <asp:Timer ID="Timer1" runat="server" Enabled="False" Interval="10000" OnTick="Timer1_Tick">
                </asp:Timer>
        
    </form>
    <div id="pie">
        <footer>
            <p>&copy; <%: DateTime.Now.Year %>- www.oasishoteles.com</p>
        </footer>

    </div>
</body>
<div id="keyboard" class="center"></div>
</html>
