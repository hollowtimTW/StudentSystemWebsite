//檢查班級科目的狀態 若為7回傳true => alert 已經開啟

function toggleStatus(id) {
    var createUrl = `/course_management/courseHome/toggleStatus/${id}`;
    $.ajax({
        type: 'GET',
        url: createUrl,
        success: function (result) {
            if (result.data) {
                alert("評分功能已經開啟!");

            } else {
                alert("開啟評分功能!");

            }
        },
        error: function (error) {
            console.error('Ajax request error', error);
        }
    });
}

