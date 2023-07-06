<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="FacebookInstaIntegrate.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>

   <%--  <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>--%>
     <link rel="shortcut icon" href="/assets/dist/img/ico/favicon.png" type="image/x-icon" />
    <link href="/css/bootstrap.min.css" rel="stylesheet" />
    
</head>
<body>
   
    <script>

        $(document).ready(function () {
            $('.btnfacebk').click(function () {
                $('#div_facebook').toggleClass('d-none');
            });
            //$('.pickshare').click(function () {
            //    $('#div_fbpickshare').removeClass('d-none');
            //});
            //$('.pickrandmcmnt').click(function () {
            //    $('#div_fbpickcmt').toggleClass('d-none');
            //});
        });

    </script>



    <form id="form1" runat="server">
       <div class=" d-inline-flex mt-4 ml-4">
            <asp:Button runat="server" id="fblogin" OnClick="fblogin_Click" Text="Login with Facebook" CssClass="btn btn-outline-success" />
           
            <div id="div_btn" runat="server" class="ml-3" visible="false" >
       
            <asp:Button runat="server" id="btnfacebk" Text="Facebook" OnClick="btnfacebk_Click" class="btn btn-outline-primary btnfacebk" />

            <input type="button" id="insta" value="Instagram" class="btn btn-outline-danger btninsta" />
          </div>
        </div>

        <div id="div_facebook"  class="mt-5" runat="server" visible="false"  >

            <div class="col-sm-12">

                 <div class="col-sm-12 mt-3">
                    <label id="Label1">Select Post:</label>
                   <asp:DropDownList runat="server" ID="ddlPageIds" AutoPostBack="true" OnSelectedIndexChanged="ddlPageIds_SelectedIndexChanged1"></asp:DropDownList>
                 </div>
                <div class="col-sm-12 mt-3">
                    <asp:Button runat="server" ID="countCmnt" OnClick="countCmnt_Click" class="btn btn-outline-primary" Text="Count Comment" />
                    <asp:Label runat="server" id="lblcntcmment"></asp:Label>
                </div>
                  <div class="col-sm-12 mt-3">
                    <asp:Button runat="server" id="pickrandmcmnt"  class="btn btn-outline-primary" OnClick="pickrandmcmnt_Click" Text="Pick Random Comment" />
                    <div id="div_fbpickcmt" class="mt-3" visible="false" runat="server">
                        <asp:Textbox  runat="server" class="" id="txtrandomcmmnt" />
                        <asp:Button runat="server" OnClick="picRandomCmnt_Click" ID="picRandomCmnt"  class="btn btn-success" Text="submit" />
                        <asp:Label runat="server" id="lblcmtshow"></asp:Label>
                    </div>
                </div>
                 <div class="col-sm-12 mt-3">
                    <asp:Button runat="server" ID="exportCmt" OnClick="exportCmt_Click" class="btn btn-outline-primary" Text="Export Comments" />
                    <asp:Label runat="server" id="lblexportCmt"></asp:Label>
                </div>
                <div class="col-sm-12 mt-3">
                    <asp:Button runat="server" ID="countLike" OnClick="countLike_Click" class="btn btn-outline-success" Text="Count Like" />
                    <asp:Label runat="server" id="lblcntlike"></asp:Label>
                     <div id="Div_like" runat="server" class="mt-3" visible="false" >
                         <label id="lblselectpost">Select Post:</label>
                   <asp:DropDownList runat="server" ID="Drpselectpost" AutoPostBack="true" OnSelectedIndexChanged="Drpselectpost_SelectedIndexChanged"></asp:DropDownList>
                   <asp:Button runat="server" OnClick="btncountlike_Click" ID="btncountlike" class="btn btn-success" Text="Submit"  value="submit" />
                         <asp:Label runat="server" id="lblcountflike"></asp:Label>
                    </div>
                </div>
                    <div class="col-sm-12 mt-3" >
                    <asp:Button runat="server"  id="PickLike" class="btn btn-outline-info mb-3" OnClick="PickLike_Click" Text="Pic Random Likes" />
                     <asp:Label runat="server" id="lblPickLike"></asp:Label>
                     <div id="div_picklike" runat="server" visible="false" >
                        <asp:Textbox  runat="server" id="txtpicklike" />
                        <asp:Button runat="server" OnClick="btnpicklike_Click" ID="btnpicklike" class="btn btn-success" Text="Submit"  value="submit" />
                      
                    </div>
                </div>
                <div class="col-sm-12 mt-3">
                    <asp:Button runat="server" ID="btnexportlike" OnClick="btnexportlike_Click" class="btn btn-outline-primary" Text="Export Likes" />
                    <asp:Label runat="server" id="lblexportlike"></asp:Label>
                </div>

                <div class="col-sm-12 mt-3">
                    <asp:Button runat="server" ID="countShare" OnClick="countShare_Click" class="btn btn-outline-secondary" Text="Count Share" />
                    <asp:Label runat="server" id="lblcntshare"></asp:Label>
                   
                </div>
                 <div class="col-sm-12 mt-3" >
                    <asp:Button runat="server"  id="pickshare" class="pickshare btn btn-outline-info mb-3" OnClick="pickshare_Click" Text="Pic Random Share" />
                    <label id="lblrandomshare"></label>
                     <div id="div_fbpickshare" runat="server" visible="false" >
                        <asp:Textbox  runat="server" class="" id="txtrandomshare" />
                        <asp:Button runat="server" OnClick="picRandomshare_Click" ID="picRandomshare" class="btn btn-success" Text="Submit"  value="submit" />
                         <asp:Label runat="server" id="lblshareshow"></asp:Label>
                    </div>
                </div>
              
            </div>
        </div>

        <div id="div_instagram" style="display: none;" class="m-5">
            <div class="col-sm-12">
                <div class="col-sm-12 mt-3">
           <input type="button" value="Count Followers" /> 
             <label id="lblcountfollwer"></label></div> <div class="col-sm-12 mt-3">
           <input type="button" value="Count Follow" /> 
             <label id="lblcountfollow"></label> </div> <div class="col-sm-12 mt-3">
           <input type="button" value="Count Like" /> 
             <label id="lblcountlike"></label></div>  <div class="col-sm-12 mt-3">
            <input type="button" value="Count Comment" /> 
             <label id="lblcountcmmt"></label></div>  <div class="col-sm-12 mt-3">
           <input type="button" value="Pick Random Comment" /> 
              <div id="div_instapickcmt" style="display: none;">
             <input type="text" id="txtrandomcommnt" />
            <input type="button" value="submit" />
  </div>
                </div>
            </div>
        </div>
    </form>


</body>
</html>
