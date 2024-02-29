
// 檢查是否存在具有 like-btn 類名的按鈕
const likeBtns = document.querySelectorAll('.like-btn');
if (likeBtns.length > 0) {
    // 如果存在，則對每個按鈕添加事件監聽器
    likeBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            // 切換按鈕的 active 狀態
            btn.classList.toggle('active');
        });
    });
}
