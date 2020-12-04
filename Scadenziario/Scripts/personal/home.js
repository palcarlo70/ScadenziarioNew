$.fn.datepicker.defaults.language = 'it';
$('.datepicker').datepicker();



$(function () {
    
    var d = new Date();

    

    //var output = (day < 10 ? '0' : '') + day + '/' +
    //    (month < 10 ? '0' : '') + month + '/' +        
    //    d.getFullYear();

    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

    var month = firstDay.getMonth() + 1;
    var day = firstDay.getDate();

    var output = (day < 10 ? '0' : '') + day + '/' +
        (month < 10 ? '0' : '') + month + '/' +
        d.getFullYear();

    $("#txtDataDa").val(output);

    month = lastDay.getMonth() + 1;
    day = lastDay.getDate();

    output = (day < 10 ? '0' : '') + day + '/' +
        (month < 10 ? '0' : '') + month + '/' +
        d.getFullYear();

    $("#txtDataA").val(output);
    popolaGruppi();


    $('input').tooltip({
        content: function () {
            return $(this).attr('title');
        }
    });

});

function checkFiltri() {
    getVoci();
}


function popolaGruppi() {
    var f = 1;

    $.ajax({
        url: "GetGruppi",
        type: "POST",
        async: true,
        dataType: "json",
        data: {},
        success: function (value) {
            //var testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckNoGruppo\" value=\"option3\" onclick=\"checkFiltri(0)\">" +
            //    " <label class=\"form-check-label\" for=\"inlineCheckbox3\" style=\"color:blue;\">No Categoria</label> </div>";
            //$("#divGruppi").prepend(testo);

            //testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckFasi\" value=\"option3\" onclick=\"checkFiltri(0)\">" +
            //    " <label class=\"form-check-label\" for=\"inlineCheckbox3\"  style=\"color:blue;\">Con Fasi</label> </div>";
            //$("#divGruppi").prepend(testo);



            var testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckEvasi\" value=\"option3\" onclick=\"checkFiltri(0)\">" +
                " <label class=\"form-check-label\" for=\"inlineCheckbox3\"  style=\"color:red;\">Evasi</label> </div>";
            $("#divGruppi").prepend(testo);

            testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckDaEvadere\" value=\"option3\" onclick=\"checkFiltri(0)\">" +
                " <label class=\"form-check-label\" for=\"inlineCheckbox3\" style=\"color:#56611b;\">Da Evadere</label> </div>";
            $("#divGruppi").prepend(testo);

            //testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckVenditaMag\" value=\"option3\" onclick=\"checkFiltri(2)\">" +
            //    " <label class=\"form-check-label\" for=\"inlineCheckbox3\" style=\"color:#56611b;\">Vendita</label> </div>";
            //$("#divGruppiMag").prepend(testo);

            $.each(value, function (index, pos) {
                try {
                    testo = "<div> <input class=\"form-check-input filtri\" type=\"checkbox\" id=\"" + pos.IdGruppo + "\" value=\"option3\"  onclick=\"checkFiltri(0)\">" +
                        " <label class=\"form-check-label\" for=\"inlineCheckbox3\">" + pos.Nome + "</label> </div>";
                    $("#divGruppi").prepend(testo);


                } catch (e) {
                    alert(e);
                }
            });

        },
        failure: function () {
            alert("Failed!");
        }
    });
}


function getVoci(IdAcquisto) {
    //noDetArt = popolo la griglia per gli accessori o per altri eventi che specifico

    $("#lblNumRecordAcquisti").html(0);


    var IdStato = "";
    var grid = "dtGridAcquisti";


    $("input.filtri:checked").each(function () {
        IdStato += $(this).attr("id") + ";";
    });


    $.ajax({
        url: "../data/GetAcquisti",
        type: "POST",
        async: true,
        dataType: "json",
        data: {
            IdAcquisto: IdAcquisto,
            IdStato: IdStato,
            IdArticolo: null,
            IdFornitore: null
        },
        success: function (value) {

            var trParo = "<tr class=\"gradeA odd text-center\" role=\"row\" style=\"font-size: 10pt !important;\" >";
            var trDisp = "<tr class=\"gradeA odd text-center\" role=\"row\" style=\"font-size: 10pt !important;\">";
            var tdOp = "<td class=\"text-left\" style=\"padding: 0 !important; vertical-align: center;\">";
            var tdCenter = "<td class=\"text-center\" style=\"padding: 0 !important;\">";
            var tdRight = "<td class=\"text-right\" style=\"padding: 0 !important;\">";
            var tdCl = "</td>";
            var rigap = "";


            if (IdAcquisto === undefined || IdAcquisto === null) {
                $("#" + grid).find("tr:not(:first)").remove();
                $("#lblNumRecordAcquisti").html(value.length);

                var conta = 0;
                $.each(value, function (index, pos) {
                    try {

                        trParo = "<tr class=\"gradeAmat odd text-center rigaSelect" + conta + "\" role=\"row\" style=\"font-size: 10pt !important;\" >";
                        trDisp = "<tr class=\"gradeAmat odd text-center rigaSelect" + conta + "\" role=\"row\" style=\"font-size: 10pt !important;\">";

                        var tr;

                        tr = trParo;

                        if (index % 2 === 1)
                            tr = trDisp;

                        rigap = tr + tdCenter + "<a href=\"#\" onClick=\"getAcquistiRda('" + pos.idAcquisto + "')\" >" + pos.Num_Doc + "</a>" + tdCl +
                            tdCenter + pos.DataOrdineString + tdCl +
                            tdOp + pos.RagSocFornitore + tdCl +
                            tdCenter + pos.Stato + tdCl +
                            tdRight + pos.TotaleStringa + tdCl +
                            tdCenter + pos.Num_DocRdo + tdCl + "</tr>";
                        // tdOp + pos.NumFigli + tdCl +



                        $("#" + grid + " tbody").append(rigap);
                        //$("#dtGridMateriali tbody").append(rigap);

                    } catch (e) {
                        alert(e);
                    }
                    conta += 1;
                });
            }

            else if (value !== null) {



                //$("#btnAddNewOrdine").attr("onclick", "savecreaRadRdo(null,null)");

                pulisciModRdoRda();

                //popolaStatoAcquisti("ddlStatoRdaRdo");
                //$("div.ddlStatoRdaRdo select").val("1");

                $("#btnPrintRdaRdo").attr("onclick", "StampaOrdineOfferta(1)");

                $("#divTitleRdaRdo").css("background-color", "#7fffd4");
                $("#lblTitleRdaRdo").html("RICHIESTA DI ACQUISTO:");

                $("div.ddlIvaRdaRdo select").val(value.IdIva);
                $("#hdIdOrdineTipoRdaRdo").val(1);
                $("#hdIdOrdineRdaRdo").val(value.idAcquisto);
                $("#hdIdOrdineFornitori").val(value.idCliFor);
                $('#btnDelOrdine').removeAttr("disabled");

                $("div.ddlValutaRda select").val(value.IdValuta);
                $("#hdValutaSimbolo").val(value.ValutaSimbolo);

                $("div.ddlStatoRdaRdo select").val(value.idStato);
                $("div.ddlModPagamentoRdaRdo select").val(value.idModoPagamento);
                $("div.ddlConsegnaRdaRdo select").val(value.IdTipoConsegna);

                if (value.NoteCortesiaIncludi === 1) $("#ckNotaCortesiaRdaRdo").prop("checked", true); else $("#ckNotaCortesiaRdaRdo").prop("checked", false);
                $("#txtNoteInGridRdaRdo").val(value.NoteCortesia);


                popolaFornitoreRdaRdo(1, value.idCliFor, value.RagSocFornitore, value.Codice, value.Via, value.Citta, value.PROV, value.CAP, value.Email, value.Referente, value.Tel, value.Cell);

                //$("#dtGridArticoliRdaRdo tbody").append(rigap);


                $("#lblNumeroRdaRdo").html(value.Num_Doc);

                /* Imponibile = !dr.IsNull("Totale") ? Convert.ToDecimal(dr["Totale"].ToString()) : (decimal?)null,
                         ImponibileStringa = !dr.IsNull("Totale") ? Convert.ToDecimal(dr["Totale"].ToString()).ToString("C") : "€ 0",
                         TotaleIva = !dr.IsNull("Totale") ? Convert.ToDecimal(dr["Totale"].ToString()) : (decimal?)null,
                         TotaleIvaStringa = !dr.IsNull("Totale") ? Convert.ToDecimal(dr["Totale"].ToString()).ToString("C") : "€ 0",
                         Totale = !dr.IsNull("Totale") ? Convert.ToDecimal(dr["Totale"].ToString()) : (decimal?)null,
                         TotaleStringa = !dr.IsNull("Totale") ? Convert.ToDecimal(dr["Totale"].ToString()).ToString("C") : "€ 0",*/




                $("#txtDataOrdineAcquisto").val(value.DataOrdineString);
                $("#txtNoteRdaRdo").val(value.Note);


                $("#divRdoRdaPrintMail").show();
                $('#myModalRDAcquisto').modal('show');

                GetRichieste("dtGridArticoliRdaRdo", null, value.idAcquisto, null);

                //$("#lblImponibileRdoRda").html(value.ImponibileStringa);
                //$("#lblImportoIvaRdoRda").html(value.TotaleIvaStringa); $("#lblImportoOrdineRdoRda").html(value.TotaleStringa);
            } else {
                alert("Nessun risultato");
            }


        },
        failure: function () {
            alert("Failed!");
        }
    });



}