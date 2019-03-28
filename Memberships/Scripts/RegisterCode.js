$(function () {

    let code = $(".register-code-panel input")
    function displayMessage(success, message) {

        let alertdiv = $(".register-code-panel .alert")

        alertdiv.text(message)
        if (success) {
            alertdiv.removeClass('alert-danger').addClass('alert-success')
        }
        else {
            alertdiv.removeClass('alert-success').addClass('alert-danger')
        }

        alertdiv.removeClass('hidden')
    }

    $(".register-code-panel button").click(function (e) {

        $(".register-code-panel .alert").addClass('hidden')
        if (code.val().length == 0) {
            displayMessage(false, "Digite o código")
            return;
        }
        $.post('/RegistrationCode/Register', { code: code.val() },
            function (data)
            {
                displayMessage(true, "Código ativado com sucesso. \n\r Por favor recarrega a página.")
                code.val('')
            }).fail(function (xlr, status, error) {

                displayMessage(false, "Código inválido")
                code.val('')
        })
    })
})