
// 定義共用的日期時間格式化方法
function formatDateTime(dateTime) {
    if (!dateTime) {
        return '不明';
    }
    const date = new Date(dateTime);
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    const hours = ('0' + date.getHours()).slice(-2);
    const minutes = ('0' + date.getMinutes()).slice(-2);
    const seconds = ('0' + date.getSeconds()).slice(-2);
    return `${year}/${month}/${day} ${hours}:${minutes}:${seconds}`;
}

document.addEventListener('DOMContentLoaded', function () {

    // 找到所有具有 .btn-like 類別的按鈕
    const likeBtns = document.querySelectorAll('.btn-like');

    // 如果存在，則對每個按鈕添加事件監聽器
    if (likeBtns.length > 0) {
        likeBtns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                // 切換按鈕的 active 狀態
                btn.classList.toggle('active');
            });
        });
    }
});



