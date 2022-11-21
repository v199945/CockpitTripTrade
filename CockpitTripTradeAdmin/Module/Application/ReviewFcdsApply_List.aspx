<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="ReviewFcdsApply_List.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Application.CockpitTripTradeAdmin_Module_Application_ReviewFcdsApply_List" %>

<%@ Register Src="~/Module/Application/UserControl/TabFcdsApplyList.ascx" TagName="TabFcdsApplyList" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <section class="mb-5">
        <ul class="nav nav-tabs  nav-justified" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <a class="nav-link active" id="allfleet-tab" data-toggle="tab" href="#allfleet" role="tab" aria-controls="allfleet" aria-selected="true">
                    All Fleet
                    <span id="AllFleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="a21nfleet-tab" data-toggle="tab" href="#a21nfleet" role="tab" aria-controls="a21nfleet" aria-selected="false">
                    A21N
                    <span id="A21NFleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="a330fleet-tab" data-toggle="tab" href="#a330fleet" role="tab" aria-controls="a330fleet" aria-selected="false">
                    A330
                    <span id="A330FleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="a350fleet-tab" data-toggle="tab" href="#a350fleet" role="tab" aria-controls="a350fleet" aria-selected="false">
                    A350
                    <span id="A350FleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="b738fleet-tab" data-toggle="tab" href="#b738fleet" role="tab" aria-controls="b738fleet" aria-selected="false">
                    B738
                    <span id="B738FleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="b744fleet-tab" data-toggle="tab" href="#b744fleet" role="tab" aria-controls="b744fleet" aria-selected="false">
                    B744
                    <span id="B744FleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="b777fleet-tab" data-toggle="tab" href="#b777fleet" role="tab" aria-controls="b777fleet" aria-selected="false">
                    B777
                    <span id="B777FleetCount" class="badge badge-pill badge-danger" runat="server" />
                </a>
            </li>
        </ul><!-- nav-tabs End -->
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane fade py-2 show active" id="allfleet" role="tabpanel" aria-labelledby="allfleet-tab">
                <uc1:TabFcdsApplyList ID="ucAllFleetTabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="a21nfleet" role="tabpanel" aria-labelledby="a21nfleet-tab">
                <uc1:TabFcdsApplyList ID="ucA21NTabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="a330fleet" role="tabpanel" aria-labelledby="a330fleet-tab">
                <uc1:TabFcdsApplyList ID="ucA330TabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="a350fleet" role="tabpanel" aria-labelledby="a350fleet-tab">
                <uc1:TabFcdsApplyList ID="ucA350TabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="b738fleet" role="tabpanel" aria-labelledby="b738fleet-tab">
                <uc1:TabFcdsApplyList ID="ucB738TabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="b744fleet" role="tabpanel" aria-labelledby="b744fleet-tab">
                <uc1:TabFcdsApplyList ID="ucB744TabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="b777fleet" role="tabpanel" aria-labelledby="b777fleet-tab">
                <uc1:TabFcdsApplyList ID="ucB777TabFcdsApplyList" runat="server" />
            </div><!-- tab-pane End-->
        </div><!-- tab-content End-->
    </section>
</asp:Content>
