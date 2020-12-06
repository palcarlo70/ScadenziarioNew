

function insertRate() {
    $("div.ddlCadenzaRate select").val(1);
    $("div.ddlGruppoRate select").val("");
    $("#txtDescriRate").val("");
    $("#txtImportoRate").val("");
    $("#txtDataRata").val("");
    $("#txtNumRate").val("");

    $('#myModalRate').modal('show');
}



function inserisciRate() {

    if ($("#txtDescriRate").val() === "" ||
        $("#txtImportoRate").val() === "" ||
        $("#txtDataRata").val() === "" ||
        $("#txtNumRate").val() === ""
    ) {
        OpenAlertmess("Popolare la DESCRIZIONE - IMPORTO - DATA - NUMERO RATE");
        return;
    }

    if ($("div.ddlGruppoRate select").val() === "" ) {
        OpenAlertmess("Scegliere il GRUPPO di apartenenza");
        return;
    }
    var dataRata = $("#txtDataRata").val();
    var descrizione = $("#txtDescriRate").val();
    var importo = $("#txtImportoRate").val();
    var idGruppo = $("div.ddlGruppoRate select").val();
    var numRate = $("#txtNumRate").val();
    var cadenza = $("div.ddlCadenzaRate select").val();

    //InsUpDelRichieste()

    $.ajax({
        url: "home/InsUpDelRichieste",
        type: "POST",
        async: false,
        dataType: "json",
        data: {
            dataRata: dataRata,
            descrizione: descrizione,
            importo: importo,
            idGruppo: idGruppo,
            numRate: numRate,
            cadenza: cadenza
        },
        success: function (value) {
            try {
                if (value[1] !== "") {//ho un errore di ritorno mostro l'errore
                    OpenAlertmess(value[1]);
                } else {
                    //aggionamento
                    $('#myModalRate').modal('hide');
                    riepilogoAnno();
                    getVoci(null,null);
                }
            } catch (e) {
                OpenAlertmess(e);
            }
        },
        failure: function () {
            OpenAlertmess("Errore generico");
        }
    });


}


function openEditVoce() {
    $("div.ddlCadenzaRate select").val(1);
    $("div.ddlGruppoRate select").val("");
    $("#txtDescriRate").val("");
    $("#txtImportoRate").val("");
    $("#txtDataRata").val("");
    $("#txtNumRate").val("");

    $('#myModalEditRate').modal('show');
}

//

//public ContentResult InsUpVoce(string dataRata, string descrizione, string importo, int idGruppo, int numRate, int cadenza, int elimina, int applicaATutti, int evaso)
function editVoce(elimina) {

    if ($("#txtDescriRate").val() === "" ||
        $("#txtImportoRate").val() === "" ||
        $("#txtDataRata").val() === "" ||
        $("#txtNumRate").val() === ""
    ) {
        OpenAlertmess("Popolare la DESCRIZIONE - IMPORTO - DATA - NUMERO RATE");
        return;
    }

    if ($("div.ddlGruppoRate select").val() === "" ) {
        OpenAlertmess("Scegliere il GRUPPO di apartenenza");
        return;
    }
    var dataRata = $("#txtDataRata").val();
    var descrizione = $("#txtDescriRate").val();
    var importo = $("#txtImportoRate").val();
    var idGruppo = $("div.ddlGruppoRate select").val();
    var numRate = $("#txtNumRate").val();
    var cadenza = $("div.ddlCadenzaRate select").val();

    //InsUpDelRichieste()

    $.ajax({
        url: "home/InsUpVoce",
        type: "POST",
        async: false,
        dataType: "json",
        data: {
            dataRata: dataRata,
            descrizione: descrizione,
            importo: importo,
            idGruppo: idGruppo,
            numRate: numRate,
            cadenza: cadenza
        },
        success: function (value) {
            try {
                if (value[1] !== "") {//ho un errore di ritorno mostro l'errore
                    OpenAlertmess(value[1]);
                } else {
                    //aggionamento
                    $('#myModalRate').modal('hide');
                    riepilogoAnno();
                    getVoci(null,null);
                }
            } catch (e) {
                OpenAlertmess(e);
            }
        },
        failure: function () {
            OpenAlertmess("Errore generico");
        }
    });


}















