$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");
    fetch("Negocio/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(data => {
            console.log(data);
            if (data.estado) {
                const d = data.objecto
                $("#txtNumeroDocumento").val(d.numeroDocumento);
                $("#txtRazonSocial").val(d.nombre);
                $("#txtCorreo").val(d.correo);
                $("#txtDireccion").val(d.direccion);
                $("#txTelefono").val(d.telefono);
                $("#txtImpuesto").val(d.porcentajeImpuesto);
                $("#txtSimboloMoneda").val(d.simboloMoneda);
                $("#imgLogo").attr("src", d.urlLogo)
            }
            else {
                swal("Los sentimos", data.mensaje, "error");
            }
        })
})


$("#btnGuardarCambios").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputsSinValor = inputs.filter(item => item.value.trim() == "");

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo: ${inputsSinValor[0].name}`;
        toastr.warning("", mensaje)
        $(`input[name=${inputsSinValor[0].name}]`).focus();
        return;
    }

    const modelo = {
        numeroDocumento: $("#txtNumeroDocumento").val(),
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: $("#txtSimboloMoneda").val()
    }

    const inputLogo = document.getElementById("txtLogo")
    const formData = new FormData();
    formData.append("logo", inputLogo.files[0]);
    formData.append("modelo", JSON.stringify(modelo))

    $(".card-body").LoadingOverlay("show");

    fetch("Negocio/GuardarCambios", {
        method: "POST",
        body: formData
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(data => {
            console.log(data);
            if (data.estado) {
                const d = data.objecto
                $("#imgLogo").attr("src", d.urlLogo)
            }
            else {
                swal("Los sentimos", data.mensaje, "error");
            }
        })

})