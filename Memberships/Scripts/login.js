$(function myfunction() {
    let loginLinkHover = $("#loginLink").hover(onLoginLinkHover);
    let loginClose = $("#close-login").click(onCloseLogin);

    function onCloseLogin() {
        $("div[data-login-user-area]").removeClass('open')
    }
    function onLoginLinkHover() {
        $("div[data-login-user-area]").addClass('open')
    }

})