<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="TaskList.aspx.cs" Inherits="CockpitTripTrade.Module.Application.CockpitTripTrade_Module_Application_TaskList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Number of Swap Request</h2><%--換班次數--%>
    <div class="row">
        <div class="col-xl-9">

            <section class="mb-5">
                <ul class="nav nav-tabs  nav-justified  nav-tabs-primary" id="myTab" role="tablist">
                    <li class="nav-item" role="presentation">
                        <a id="tab_currentmonth" class="nav-link active" data-toggle="tab" href="#tabpanel-currentmonth" role="tab" aria-controls="tab-currentmonth" aria-selected="true" runat="server"></a><%--Sep 2020--%>
                    </li>
                    <li class="nav-item" role="presentation">
                        <a id="tab_nextmonth" class="nav-link" data-toggle="tab" href="#tabpanel-nextmonth" role="tab" aria-controls="tab-nextmonth" aria-selected="false" runat="server"></a><%--Oct 2020--%>
                    </li>
                </ul><!-- nav-tabs End -->
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active" id="tabpanel-currentmonth" role="tabpanel" aria-labelledby="tab-currentmonth">
                        <section class="switchcount  in-tabpanel">
                            <div class="switchcount-head">
                                <div class="switchcount-avail">
                                    <asp:Label ID="lblCurrentMonthRemaining" CssClass="switchc-countNo" runat="server" /><%--<span class="switchc-countNo">2</span>--%>
                                    <p>Number of Remaining</p><%--尚餘次數--%>
                                </div>
                                <div class="switchcount-used">
                                    <asp:Label ID="lblCurrentMonthUsed" CssClass="switchc-countNo" runat="server" /><%--<span class="switchc-countNo">1</span>--%>
                                    <p>Number of Used (including In Process)</p><%--已使用(含呈核中)--%>
                                </div>
                            </div>
                            <div class="switchcount-body">
                                <asp:HyperLink ID="hlNewCurrentMonthFcdsApply" CssClass="btn  btn-primary-sp" Text="New Request" type="button" runat="server" />
                                <%--<asp:Button ID="btnNewCurrentMonthFcdsApply" CausesValidation="false" CssClass="btn  btn-primary-sp" Enabled="false" Text="New" runat="server" />--%>
                                <%--<a type="button" class="btn  btn-primary-sp" href="填寫申請單_step01_選擇受申請人.html">申請換班</a>--%>
                            </div>
                        </section>
                    </div><!-- tab-pane End--> 
                    <div class="tab-pane fade " id="tabpanel-nextmonth" role="tabpanel" aria-labelledby="tab-nextmonth">
                        <section class="switchcount  in-tabpanel">
                            <div class="switchcount-head">
                                <div class="switchcount-avail">
                                    <asp:Label ID="lblNextMonthRemaining" CssClass="switchc-countNo" runat="server" /><%--<span class="switchc-countNo">3</span>--%>
                                    <p>Number of Remaining</p><%--尚餘次數--%>
                                </div>
                                <div class="switchcount-used">
                                    <asp:Label ID="lblNextMonthUsed" CssClass="switchc-countNo" runat="server" /><%--<span class="switchc-countNo">0</span>--%>
                                    <p>Number of Used (including In Process)</p><%--已使用(含呈核中)--%>
                                </div>
                            </div>
                            <div class="switchcount-body">
                                <asp:HyperLink ID="hlNewNextMonthFcdsApply" CssClass="btn  btn-primary-sp" Text="New Request" type="button" runat="server" />
                                <%--<a type="button" class="btn  btn-primary-sp" href="填寫申請單_step01_選擇受申請人.html">申請換班</a>--%>
                            </div>
                        </section>   
                    </div><!-- tab-pane End--> 
                </div><!-- tab-content End-->
            </section>

        </div><!-- col-md-8 -->
    </div><!-- row -->

    <h2>In Process：<asp:Label ID="lblInProcess" CssClass="text-danger ml-1 mr-1" runat="server" />Form(s)</h2><%--流程中表單 2 張--%>
    <div class="row">
        <div class="col-xl-9">
            <asp:Repeater ID="repInProcess" runat="server" OnItemDataBound="repInProcess_ItemDataBound">
                <HeaderTemplate>

                </HeaderTemplate>
                <ItemTemplate>
                    <!--單張表單區塊開始-->
                    <section class="sheetcurrentstatus">
                        <header class="scs-header">
                            <p>
                                <span class="scs-infolabel">Form No.</span>
                                <span class="scs-sheet-id">
                                    <asp:HyperLink ID="hlIDFcdsApply" Text='<%# Eval("IDFcdsApply") %>' runat="server" />
                                </span>
                            </p>
                        </header>
                        <div class="scs-body">        
                            <ol class="scs-showsteps">
                                <li class="scs-stepitem">
                                    <div class="scs-stepball">1</div>
                                    <div class="scs-step-info">
                                        <div class="scs-userid"><asp:Label ID="lblApplicantID" Text='<%# Eval("ApplicantID") %>' runat="server" /><%--<span>600321</span>--%></div>
                                        <div class="scs-name"><asp:Label ID="lblApplicant" runat="server" /><%--<span>Micheal Korsss Abstract</span>--%></div>
                                        <div class="scs-date"><asp:Label ID="lblApplicantDate" runat="server" /><%--<span>09Sep</span>--%> Applied</div>
                                    </div>
                                </li>
                                <li id="liRespondent" class="scs-stepitem" runat="server"><%--   active--%>
                                    <div class="scs-stepball">2</div>
                                    <div class="scs-step-info">
                                        <div id="divRespondentID" runat="server"></div><%--本人--%>
                                        <div id="divRespondent" class="scs-name" runat="server"></div>
                                        <div class="scs-reviewstatus"><span>Confirming</span><%--審核中--%></div>
                                    </div> 
                                </li>
                                <li id="liOP" class="scs-stepitem" runat="server">
                                    <div class="scs-stepball">3</div> 
                                    <div class="scs-step-info">
                                        <div>Crew Scheduling Dept.</div><%--組派部--%>
                                        <div class="scs-reviewstatus"><span>Reviewing</span><%--審核--%></div>
                                    </div> 
                                </li>
                                <li id="liFinished" class="scs-stepitem" runat="server">
                                    <div class="scs-stepball">4</div>
                                    <div class="scs-step-info">
                                        <div class="scs-reviewstatus"><span>Finished</span><%--完成--%></div>
                                    </div> 
                                </li>  
                            </ol>
                        </div>
                    </section>
                    <!--單張表單區塊結束-->
                </ItemTemplate>
            </asp:Repeater>

        </div><!-- col-md-8 -->
    </div><!-- row -->
</asp:Content>
