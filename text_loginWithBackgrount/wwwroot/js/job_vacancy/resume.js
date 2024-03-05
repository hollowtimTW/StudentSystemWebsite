const btnSummit = document.getElementById('btnSummit');
const studentName = document.getElementById('Name');
const resumeTitle = document.getElementById('ResumeTitle');
const salaryInput = document.getElementById('salaryInput');
const hopeSalary3 = document.getElementById('hopeSalary3');
const Photo = document.getElementById('Photo');

document.addEventListener('DOMContentLoaded', function () {

    // 定義一個函式，用於預覽上傳的圖片
    function previewImage(inputFile, img) {
        if (inputFile.files && inputFile.files[0]) {
            let allowType = "image.*";
            // 檢查上傳的檔案類型是否符合允許的檔案類型
            if (inputFile.files[0].type.match(allowType)) {
                // 創建 FileReader 物件用於讀取檔案
                let reader = new FileReader();
                reader.onload = function (e) {
                    $(img).attr('src', e.target.result); // 將 img 包裝在 $() 函數中
                    $(img).attr('title', inputFile.files[0].name); // 將 img 包裝在 $() 函數中
                    //$(".btn").prop('disabled', false);
                };
                // 開始讀取檔案內容，這會觸發上述的 reader.onload 事件
                reader.readAsDataURL(inputFile.files[0]);
            } else {
                alert("不支援的檔案上傳格式！");
                //$(".btn").prop('disabled', true);
            }
        }
    }

    //顯示圖片
    Photo.addEventListener('change', function (event) {
        var inputFile = event.target;
        var img = document.getElementById('previewImage');
        previewImage(inputFile, img);
    });


    //顯示自傳編輯區
    $('#summernote').summernote({
        placeholder: '請確認自傳中未填寫到個人隱私資料，以確保您的個人資料安全。',
        tabsize: 2,
        height: 120,
        lang: 'zh-TW',
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'picture', 'video']],
            // ['view', ['fullscreen', 'codeview', 'help']]
        ]
    });
});

//自訂薪資欄位
document.addEventListener("DOMContentLoaded", function () {
    // 當選擇"自訂薪資"時，顯示薪資輸入框
    hopeSalary3.addEventListener("change", function () {
        if (this.checked) {
            salaryInput.style.visibility = "visible";
        }
    });

    // 當選擇其他選項時，隱藏薪資輸入框
    const otherRadios = document.querySelectorAll('input[name="HopeSalary"]:not(#hopeSalary3)');
    otherRadios.forEach(function (radio) {
        radio.addEventListener("change", function () {
            salaryInput.style.visibility = "hidden";
        });
    });
})