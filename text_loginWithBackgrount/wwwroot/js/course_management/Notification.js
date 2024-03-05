$(function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();
    connection.on("ReceiveNotification", function (message) {
        //顯示有通知紅點
        var spanElement = document.createElement('span');
        spanElement.classList.add('count');
        var bellIcon = document.querySelector('.bx.bx-bell');
        bellIcon.parentNode.insertBefore(spanElement, bellIcon.nextSibling);

        //顯示通知               
        alert(message);
    });


    connection.start()
        .then(function () {
            console.log('SignalR connection established.');
            return connection.invoke("GetConnectionId");
        })
        .then(function (connectionId) {
            console.log('Connection ID: ' + connectionId);
            sessionStorage.setItem('connectionId', connectionId);
        })
        .catch(function (err) {
            return console.error(err.toString());
        });

  
});

$(function () {
    $.ajax({
        url: '/course_management/courseHome/notificationState/' + '@User?.Claims.FirstOrDefault(c => c.Type == "teacherID").Value',
        type: 'GET',
        success: function (response) {
            if (response) {
                var spanElement = document.createElement('span');
                spanElement.classList.add('count');
                var bellIcon = document.querySelector('.bx.bx-bell');
                bellIcon.parentNode.insertBefore(spanElement, bellIcon.nextSibling);
            } else {
                var countElement = document.querySelector('.count');
                if (countElement) {
                    countElement.parentNode.removeChild(countElement);
                }
            }
        },
        error: function (error) {
            console.error(error);
        }
    });
});

