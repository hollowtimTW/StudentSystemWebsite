//彈跳視窗的title
let exampleModalLabel = document.getElementById("exampleModalLabel");

$(document).ready(function () {

    // 在畫面初始時就先直接Datatable初始化
    $("table").dataTable({
        ajax: {
            type: "GET",
            url: "/course_management/T課程科目分類/IndexJson",
            dataSrc: function (json) { return json; },
            //錯誤處理
            error: function (xhr, textStatus, errorThrown) {
                console.error("Error loading data:", errorThrown);
                alert("Failed to load data. Please try again later.");
            }
        },
        columns: [
            { "data": "科目類別名稱", "width": "30%" },
            {
                "data": "科目類別封面", "width": "40%",
                "render": function (data, type, row) {
                    if (data) {
                        return '<img src="/images/t課程科目類別/' + data + '" style="max-width:120px; max-height:90px;">';
                    } else {
                        //"沒有照片"的圖片，但後端會幫忙塞默認照片
                        return '<img src="https://t4.ftcdn.net/jpg/04/73/25/49/360_F_473254957_bxG9yf4ly7OBO5I0O5KABlN930GwaMQz.jpg" style="max-width:120px; max-height:90px;">';
                    }
                }
            },
            {
                "data": "科目類別id",
                "render": function (data, type, row) {
                    return '<p><a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_edit(' + row.科目類別id + ')"><i class="bx bx-edit-alt"></i></a></p>';
                },
                "orderable": false, "width": "10%"
            },
            {
                "data": "科目類別id",
                "render": function (data, type, row) {
                    return '<p><a class="btn btn-warning" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_delete(' + row.科目類別id + ')"> <i class="bx bx-trash"></i> </a></p>';
                },
                "orderable": false, "width": "10%"
            },
            {
                "data": "科目類別id",
                "render": function (data, type, row) {
                    return '<p><a class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_detail(' + row.科目類別id + ')"><i class="bx bx-menu" ></i></a></p>';
                },
                "orderable": false, "width": "10%"
            }
        ],
    });

    // 在畫面初始時就先直接綁定事件，.img_click點擊時件會下在該圖片， 在fetchUnsplash裡  img.classList.add("img_click");
    // 當點到照片就會透過downloadImage 方法下載下來
    $(document).on("click", ".img_click", function (event) {
        downloadImage(event);
    });

});

//事件區
//關閉彈跳視窗的事件
$('#exampleModal').on('hidden.bs.modal', function (e) {
    closeModal();
});

//方法區

//關閉彈跳視窗的方法
function closeModal() {
    exampleModalLabel.innerText = "Modal title";
}

//create的方法
function popup_create() {
    exampleModalLabel.innerText = "Create";
    $.ajax({
        type: 'GET',
        url: '/course_management/T課程科目分類/create',
        success: function (result) {
            console.log('Ajax create request success');
            //彈跳視窗的內容更換
            $("#switch_context").html("");
            $("#switch_context").html(result);
        },
        error: function (error) {
            console.error('Ajax request error', error);
        }
    });
}

//detail的方法
function popup_detail(id) {
    exampleModalLabel.innerText = "Details";

    $.ajax({
        type: 'GET',
        url: '/course_management/T課程科目分類/Details/' + id,
        success: function (result) {
            console.log('Ajax request success');
            $("#switch_context").html("");
            $("#switch_context").html(result);
        },
        error: function (error) {
            console.error('Ajax request error', error);
        }
    });
}

function popup_edit(id) {
    $.ajax({
        type: 'GET',
        url: '/course_management/T課程科目分類/Edit/' + id,
        success: function (result) {
            console.log('Ajax request success');
            $("#switch_context").html("");
            $("#switch_context").html(result);
        },
        error: function (error) {
            console.error('Ajax request error', error);
        }
    });
}

//delete的方法
function popup_delete(id) {
    exampleModalLabel.innerText = "Delete";

    $.ajax({
        type: 'GET',
        url: '/course_management/T課程科目分類/Delete/' + id,
        success: function (result) {
            console.log('Ajax delete request success');
            $("#switch_context").html("");
            $("#switch_context").html(result);
        },
        error: function (error) {
            console.error('Ajax request error', error);
        }
    });
}


//預覽照片的方法，只決定是否可見，長寬寫在html
function previewImage(event) {
    const file = event.target.files[0];
    const reader = new FileReader();

    reader.onload = function () {
        const preview = document.getElementById('preview');
        preview.src = reader.result;
        preview.style.display = 'block';
    }

    if (file) {
        reader.readAsDataURL(file);
    }
}

//使用api第一步:輸入關鍵字獲取相關照片
function fetchUnsplash() {
    var query = document.getElementById('queryInput').value;
    fetch(`https://api.unsplash.com/search/photos/?query=${query}&client_id=mMuwY1LC33QCOEAdhv8u-ieuDRTHs6qja0iMfyZNeUY`)
        .then((res) => {
            return res.json();
        })
        .then((data) => {
            let container = document.querySelector("#unplash_context");
            let row;
            container.innerHTML = '';

            //只取四張所以i<4
            for (let i = 0; i < 4; i++) {
                //當i==0 || i== 2(左列第一排) 就創建div成為一行div 
                if (i % 2 === 0) {
                    row = document.createElement("div");
                    row.classList.add("row", "mt-2");
                    container.appendChild(row);
                }
                //每一張照片都是col-md-6、text-center 的一欄
                let col = document.createElement("div");
                col.classList.add("col-md-6");
                col.classList.add("text-center");

                //每一張照片都是img html元素、src是small size、設置圖片網址以利後面綁定事件能加以下載、180 x120 size
                let img = document.createElement("img");
                img.setAttribute("src", data.results[i].urls.small);
                img.setAttribute("data-url", data.results[i].urls.small);
                img.setAttribute("width", "180");
                img.setAttribute("height", "120");
                img.classList.add("img_click");  
                //img 附加在剛剛的創建欄
                col.appendChild(img);

                //欄位附加載剛剛創建的row
                row.appendChild(col);
            }
        })
        .catch(error => console.error('Fetch Unsplash error:', error));
}

//使用api第二步:每張照片被點會觸發事件，根據照片網址下載照片方法
function downloadImage(event) {
    //照片的網址
    var imageUrl = event.target.getAttribute("data-url");

    // 發送 AJAX 請求下載圖片
    fetch(imageUrl).then(response => {
        return response.blob()
            .then(blob => {
                //獲取照片數據流的大小和擴展名
                const contentType = response.headers.get("content-type");
                const extension = contentType.split("/")[1];
                return { blob: blob, extension: extension };
            })
            .then(data => {
                const extension = data.extension;
                if (data.blob.size === 0) {
                    throw new Error("下載的圖片大小為0");
                }
                const fileName = `downloaded_image.${extension}`;
                var fileInput = document.getElementById('fileInput');
                var file = new File([data.blob], fileName);
                // 覆蓋 fileInput.files[0]，DataTransfer object是因為這樣才能更新到input的file
                var fileList = new DataTransfer();
                fileList.items.add(file);
                fileInput.files = fileList.files;
                // 觸發 change 事件以更新文件輸入元素的狀態
                fileInput.dispatchEvent(new Event('change'));
            })
            .catch(error => console.error('下載圖片時發生錯誤:', error));
    });
}

//使用api的區域要不要顯示
function toggleUnsplash() {
    var unsplashDiv = document.getElementById("unsplashDiv");
    if (unsplashDiv.style.display === "none") {
        unsplashDiv.style.display = "block";
    } else {
        unsplashDiv.style.display = "none";
    }
}


//edit方法中預覽原本照片變成後來更新的照片方法，被綁定在"editpartial"科目封面input 的onchange="previewImageEdit(event)"
function previewImageEdit(event) {
    var original_picture = document.getElementById("original_picture");
    original_picture.style.display = "none";

    const file = event.target.files[0];
    const reader = new FileReader();

    reader.onload = function () {
        const preview = document.getElementById('preview');
        preview.src = reader.result;
        preview.style.display = 'block';
    }

    if (file) {
        reader.readAsDataURL(file);
    }
}


