//import { htmlimports } from "modernizr";

$.fn.datepicker.defaults.language = 'it';
$('.datepicker').datepicker();



$(function () {

    $('input:text.soloNumeriNegativi').keypress(function (e) {
        if (e.which !== 8 && e.which !== 0 && (e.which < 45 || e.which > 58) && e.which !== 46 && e.which !== 47) {
            return false;
        }
        else if (e.which === 45) {
            var contr = (this.value.match(new RegExp("-", "g")) || []).length;
            if (contr > 0) return false;
        }
    });

    $('input:text.soloNumeriDecimali').keypress(function (e) {
        if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 58) && e.which !== 46 && e.which !== 47) {
            return false;
        }
        //else if (e.which === 45) {
        //    var contr = (this.value.match(new RegExp("-", "g")) || []).length;
        //    if (contr > 0) return false;
        //}
    });

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

    //Popolamento combo ANNO

    //$("#ddlMese").append($('<option>').val(1).text('GEN'));
    //$("#ddlMese").append($('<option>').val(2).text('FEB'));
    //$("#ddlMese").append($('<option>').val(3).text('MAR'));
    //$("#ddlMese").append($('<option>').val(4).text('APR'));



    $("#ddlAnno").append($('<option>').val(d.getFullYear() - 1).text(d.getFullYear() - 1));

    for (i = 0; i < 10; i++) {
        $("#ddlAnno").append($('<option>').val(d.getFullYear() + i).text(d.getFullYear() + i));
    }
    //$("div.ddlAnno select").val("2020");
    $("div.ddlAnno select").val(d.getFullYear());
    $("div.ddlMese select").val(month);

    
    

    $("#hdDataVoci").val($("div.ddlMese select").val() + '/' + $("div.ddlAnno select").val());

    $('input').tooltip({
        content: function () {
            return $(this).attr('title');
        }
    });
    riepilogoAnno();
    getVoci();
});

function checkFiltri() {
    getVoci();
}

function pulisciCombo(ddl) {
    if (ddl !== undefined && ddl !== null)
        $("#" + ddl)
            .find('option')
            .remove()
            .end();
}

function popolaGruppi() {
    var f = 1;


    pulisciCombo("ddlGruppoRate");
    $("#ddlGruppoRate").append($('<option>').val("").text(""));

    $.ajax({
        url: "home/GetGruppi",
        type: "POST",
        async: true,
        dataType: "json",
        data: {},
        success: function (value) {
            var testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckEvasi\" value=\"option3\" onclick=\"checkFiltri(0)\">" +
                " <label class=\"form-check-label\" for=\"inlineCheckbox3\"  style=\"color:red;\">Evasi</label> </div>";
            $("#divGruppi").prepend(testo);

            testo = "<div> <input class=\"form-check-input\" type=\"checkbox\" id=\"ckDaEvadere\" value=\"option3\" onclick=\"checkFiltri(0)\">" +
                " <label class=\"form-check-label\" for=\"inlineCheckbox3\" style=\"color:#56611b;\">Da Evadere</label> </div>";
            $("#divGruppi").prepend(testo);

            $.each(value, function (index, pos) {
                try {

                    $("#ddlGruppoRate").append($('<option>').val(pos.IdGruppo).text(pos.Nome));

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


function getVoci(idVoce, giorno) {
    //noDetArt = popolo la griglia per gli accessori o per altri eventi che specifico

    $("#lblNumRecord").html(0);
    $("#lblTotPeriodo").html("0 €");
    var grid = "dtGridVoci";

    var gruppo = "";
    $("input.filtri:checked").each(function () {
        gruppo += $(this).attr("id") + ";";
    });

    var dataGiorno = $("#hdDataVoci").val(); //$("div.ddlMese select").val() + '/' + $("div.ddlAnno select").val();

    if (giorno != undefined && giorno != null) { dataGiorno = giorno; $("#hdDataVoci").val(giorno);}

    $("#lblPeriodo").html(dataGiorno);

    var descri = $("#txtRicerca").val();
    var evaso = null;
    var daEvadere = null;
    //var idVoce = null;
    $.ajax({
        url: "home/GetVoci",
        type: "POST",
        async: true,  
        dataType: "json",
        data: {
            idVoce: idVoce,
            gruppo: gruppo,
            dataGiorno: dataGiorno,
            descri: descri,
            evaso: evaso,
            daEvadere: daEvadere,
        },
        success: function (value) {

            var trParo = "<tr class=\"gradeA odd text-center\" role=\"row\" style=\"font-size: 10pt !important;\" >";
            var trDisp = "<tr class=\"gradeA odd text-center\" role=\"row\" style=\"font-size: 10pt !important;\">";
            var tdOp = "<td class=\"text-left\" style=\"padding: 0 !important; vertical-align: center;\">";
            var tdCenter = "<td class=\"text-center\" style=\"padding: 0 !important;\">";
            var tdRight = "<td class=\"text-right\" style=\"padding: 0 !important;\">";
            var tdCl = "</td>";
            var rigap = "";


            //if (IdAcquisto === undefined || IdAcquisto === null) {
            $("#" + grid).find("tr:not(:first)").remove();


            if (value === null) {
                OpenAlertmess("Attenzione Errore nella procedura GetVoci");
                return;
            }

            //ImportoMedioGiorno
            
            $("#lblTotPeriodo").html(value.ImportoTot);
            $("#lblNumRecord").html(value.Voci.length);
            var conta = 0;
            $.each(value.Voci, function (index, pos) {
                try {

                    trParo = "<tr class=\"gradeAmat odd text-center rigaSelect" + conta + "\" role=\"row\" style=\"font-size: 10pt !important;\" >";
                    trDisp = "<tr class=\"gradeAmat odd text-center rigaSelect" + conta + "\" role=\"row\" style=\"font-size: 10pt !important;\">";

                    var tr;
                    tr = trParo;

                    if (index % 2 === 1)
                        tr = trDisp;


                    rigap = tr + tdOp + "<a href=\"#\" onClick=\"openEditVoce('" + pos.IdVoce + "')\" >" + pos.Descrizione + "</a>" + tdCl +
                        tdOp + pos.Gruppo + tdCl +
                        tdCenter + pos.ScadenzaStringa + tdCl +
                        tdRight + pos.ImportoStringa + tdCl + "</tr>";
                    
                    $("#" + grid + " tbody").append(rigap);                    

                } catch (e) {
                    alert(e);
                }
                conta += 1;
            });

            grid = "dtGridVociCompresse";

            $("#" + grid).find("tr:not(:first)").remove();
            $("#lblNumRecordGiorni").html(value.VociCompresse.length);

            conta = 0;
            $.each(value.VociCompresse, function (index, pos) {
                try {

                    trParo = "<tr class=\"gradeAmat odd text-center rigaSelect" + conta + "\" role=\"row\" style=\"font-size: 10pt !important;\" >";
                    trDisp = "<tr class=\"gradeAmat odd text-center rigaSelect" + conta + "\" role=\"row\" style=\"font-size: 10pt !important;\">";

                    var tr;

                    tr = trParo;

                    if (index % 2 === 1)
                        tr = trDisp;

                    /*
                     public string Giornata { get; set; } 
        public string Totale { get; set; }
        public string TolTipString { get; set; }
                     */


                    rigap = tr + tdOp + "<a href=\"#\" onClick=\"\" title=\"" + pos.TolTipString + "\" >" + pos.Giornata + "</a>" + tdCl +
                        tdRight + pos.Totale + tdCl + "</tr>";




                    $("#" + grid + " tbody").append(rigap);


                } catch (e) {
                    alert(e);
                }
                conta += 1;
            });



        },
        failure: function () {
            alert("Failed!");
        }
    });



}


function riepilogoAnno() {
    var giorno = '1/' + $("div.ddlMese select").val() + '/' + $("div.ddlAnno select").val();

    $.ajax({
        url: "home/GetRiepilogoAnnuale",
        type: "POST",
        async: true,
        dataType: "json",
        data: {
            giorno: giorno
        },
        success: function (value) {

            var trParo = "<tr class=\"gradeA odd text-center\" role=\"row\" style=\"font-size: 10pt !important;\" >";
            var trDisp = "<tr class=\"gradeA odd text-center\" role=\"row\" style=\"font-size: 10pt !important;\">";
            var tdOp = "<td class=\"text-left\" style=\"padding: 0 !important; vertical-align: center;\">";
            var tdCenter = "<td class=\"text-center\" style=\"padding: 0 !important;\">";
            var tdRight = "<td class=\"text-right\" style=\"padding: 0 !important;\">";
            var tdCl = "</td>";
            var rigap = "";

            var grid = "dtGridAnno";

            //$("#" + grid).find("tr:not(:first)").remove();
            $("#" + grid).find("tr").remove();

            //Carico i titoli della griglia
            var conta = 0;

            var titolo = "<tr style=\"font-size: 9pt!important; \"> <td class=\"text-center\">Gruppo</td>";
            $.each(value.Titoli, function (index, pos) {
                try {
                    titolo += "<td class=\"text-center\">" + "<a href=\"#\" onClick=\"getVoci(null, '" + pos.Giorno + "')\"  >" + pos.Titolo + "</a>" + "</td>";                   

                } catch (e) {
                    alert(e);
                }
                conta += 1;
            });

            $("#" + grid + " thead").append(titolo + "</tr>");



            $("#" + grid).find("tr:not(:first)").remove();


            conta = 0;
            $.each(value.Riepilogo, function (index, pos) {
                try {

                    trParo = "<tr class=\"text-center\" role=\"row\" style=\"font-size: 10pt !important;\" >";
                    trDisp = "<tr class=\"text-center\" role=\"row\" style=\"font-size: 10pt !important;\">";

                    if (pos.Tipo == 1) {                          
                        trParo = "<tr class=\"text-center\" role=\"row\" style=\"font-size: 10pt !important; background-color:#d3fbc6;\" >";
                        trDisp = "<tr class=\"text-center\" role=\"row\" style=\"font-size: 10pt !important; background-color:#faebd7;\">";                        
                    }

                    var tr;
                    tr = trParo;
                    if (index % 2 === 1)
                        tr = trDisp;       
                    
                    if (pos.Gruppo === 'Totale') {
                        tdRight = "<td class=\"text-right\" style=\"padding: 0 !important; color:red; \"> <b>";
                    } else if (pos.Gruppo === 'ImpDaEvadere') {
                        tdRight = "<td class=\"text-right\" style=\"padding: 0 !important; color:blue; \"> <b>";
                    }else {
                        tdRight = "<td class=\"text-right\" style=\"padding: 0 !important;\">";
                    }

                    rigap = tr + tdOp + "<b>" + pos.Gruppo + "</b>" +tdCl +
                        tdRight + pos.Mese1 + tdCl +
                        tdRight + pos.Mese2 + tdCl +
                        tdRight + pos.Mese3 + tdCl +
                        tdRight + pos.Mese4 + tdCl +
                        tdRight + pos.Mese5 + tdCl +
                        tdRight + pos.Mese6 + tdCl +
                        tdRight + pos.Mese7 + tdCl +
                        tdRight + pos.Mese8 + tdCl +
                        tdRight + pos.Mese9 + tdCl +
                        tdRight + pos.Mese10 + tdCl +
                        tdRight + pos.Mese11 + tdCl +
                        tdRight + pos.Mese12 + tdCl + "</tr>";




                    $("#" + grid + " tbody").append(rigap);


                } catch (e) {
                    alert(e);
                }
                conta += 1;
            });



        },
        failure: function () {
            alert("Failed!");
        }
    });

}


function OpenAlertmess(mess) {
    $("#lblMessaggioAllert").text(mess);
    $("#myModalMessageAllert").modal("show");

}