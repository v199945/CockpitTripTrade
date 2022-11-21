<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exception.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.ErrorHandler.Exception" %>

<!DOCTYPE html>

<html class="" lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="keywords" content="網頁關鍵字" />
    <meta name="description" content="這裡是網頁的簡短描述" />
    <meta name="copyright" content="中華航空 China Airlines" />
    <title>Flight Crew Duty Swap Application System</title>

    <!--css-->
    <link rel="stylesheet" type="text/css" href="../../asset/v3.0.0/css/civd-content-style.css?vd20210121" />
    <link rel="stylesheet" type="text/css" href="../../asset/v3.0.0/css/temp-otherstate-v3s.css?vd20210121" />
    <link rel="stylesheet" type="text/css" href="../../asset/v3.0.0/css/page-login-v3s-fcds.css" />

    <script src="../../asset/v3.0.0/js/jquery.min.js?vd20210121"></script>
    <script src="../../asset/v3.0.0/js/bootstrap.bundle.min.js?vd20210121"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="wrapper">
 
	        <div class="containerblock">
    	        <div id="block01">
                    <header id="block01-head">
                      <div id="ci-logo">
                        <img src="../../asset/v3.0.0/images/img-temp-conblock-v3s/img-ci-logo-en.png" 
                                srcset="../../asset/v3.0.0/images/img-temp-conblock-v3s/img-ci-logo-en.png 1x,
                                        ../../asset/v3.0.0/images/img-temp-conblock-v3s/img-ci-logo-en@2x.png 2x,
                                        ../../asset/v3.0.0/images/img-temp-conblock-v3s/img-ci-logo-en@3x.png 3x"
                                        id="ci-logo-img"  alt="China Airlines Logo" />
                      </div>
            
                      <div id="page-title">
                          <h1 id="page-title-mainlang" class="fcd-title">Flight Crew Duty Swap Application System</h1>
                          <!-- <h1 id="page-title-mainlang" class="pt-tw-text10">中文十字標題管理系統</h1>
                          <p id="page-title-sublang" class="">English Title Management System</p> -->
                      </div>       
                    </header>

            
                    <div id="block01-body">         
                        <p  class="text-center">
                            <img src="../../asset/v3.0.0/images/img-temp-conblock-v3s/status-maintaince.svg" alt="" />
                        </p>
                    </div><!--block-body End-->
                </div><!-- block01 End -->

                <div id="block02" class="">
                    <h2 class="mt-0  text-center">System Exception</h2>
                    <p class="text-center">There is an abnormal situation in the system, we will fix it as soon as possible. If you have any further questions, please contact your system administrator.</p>
                    <p class="text-center  mt-4  mb-5">
                        <asp:HyperLink ID="hlLogin" CssClass="btn btn-primary-sp" NavigateUrl="~/Login.aspx" Text="Back" runat="server" />
                        <%--<input  id=""  type="button" class="btn btn-primary-sp" value="重新登入" />--%>
                    </p>
                </div>
            </div><!--Main End -->
    
	        <div id="footer">
    	        <!-- 如果有頁尾內容，可寫在這 -->
            </div><!--footer End -->
    
        </div><!--Wrapper End -->

        <div id="htmlversion">CITemplateV3-login</div>
        <!--htmlversion: for Reference, Dont Delete this 檢查HTML版本註記用, 不會出現，不要把我刪掉-->
    </form>
</body>
</html>
