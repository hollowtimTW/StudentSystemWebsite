$(document).ready(function () {
    //全域變數，代表被選到的班級科目
    var selectedSubjects = [];

    // step1的button，不會送資訊到後端，會檢查每個欄位
    $(document).on('click', '#next', function (e) {

        e.preventDefault();

        var className = $("[data-id='班級名稱']").val();
        var teacherId = $("[data-id='班級導師id']").val();
        var admissionDate = $("[data-id='入學日期']").val();
        var graduationDate = $("[data-id='結訓日期']").val();

        if (className == "" || teacherId == "" || admissionDate == "" || graduationDate == "") {
            alert("請填寫所有字段。");
            return false;
        } else if (admissionDate >= graduationDate) {
            alert("請正確填寫時間。");
            return false;
        } else {
            $("#progressBar").css("width", "50%");
            $("#title").text("step2:選擇班級科目");
            $("#step1").hide();
            $("#step2").show();
        }
    });

    // demo的button，填寫好資料
    $(document).on('click', '#demo', function (e) {

        e.preventDefault();

        $("[data-id='班級名稱']").val("msitDemoTest01");
        $("[data-id='班級導師id']").val("1");
        $("[data-id='入學日期']").val("2024-04-20T00:00");
        $("[data-id='結訓日期']").val("2024-09-20T00:00");       
    });

    //step2的button，和後端互動的表單由此送出
    $("#submit").on('click', function (e) {
        e.preventDefault();

        var formData = {
            班級名稱: $("[data-id='班級名稱']").val(),
            班級導師id: $("[data-id='班級導師id']").val(),
            入學日期: $("[data-id='入學日期']").val(),
            結訓日期: $("[data-id='結訓日期']").val(),
            班級科目: selectedSubjects,
            狀態: 1
        };

        $.ajax({
            url: "/course_management/T班級創建/Create",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                console.log("Success:", response);
                alert("成功");
                window.location.href = '/course_management/T課程班級/Index';
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                alert("發生錯誤，請稍後再試。");
            }
        });
    });

    //取得每個科目的授課老師列表
    $('#subjectSelect').change(function () {
        var selectedSubjectId = $(this).val();
        $.ajax({
            url: '/course_management/T班級創建/GetTeachersBySubjectId/' + selectedSubjectId,
            type: 'GET',
            success: function (data) {
                console.log(data);
                if (data && data.length > 0) {

                    $('#TeacherList').empty();
                    $.each(data, function (index, teacher) {
                        // 為每位老師創建相應的按鈕
                        var button = $('<button>').text('選擇').addClass('btn btn-primary selectTeacher')
                            .attr({
                                'data-teacher-id': teacher.老師id,
                                'data-course-id': selectedSubjectId
                            });
                        // 創建表格行，有兩欄，一欄顯示老師姓名和一欄選擇按鈕
                        var row = $('<tr>').append($('<td>').text(teacher.姓名)).append($('<td>').append(button));
                        $('#TeacherList').append(row);
                    });
                } else {
                    $('#TeacherList').empty();
                    var row = $('<tr>').append($('<td>').addClass('text-center').text("目前沒有授課老師"));
                    $('#TeacherList').append(row);
                }
            },
            error: function () {
                alert('Failed to retrieve teachers.');
            }
        });
    });


    

    //刪除陣列
    $(document).on('click', '#subjectTeacherList button', function (e) {
        e.preventDefault();
        //被點擊到的就是this
        var teacherId = $(this).closest('li').attr('data-teacher-id');
        var courseId = $(this).closest('li').attr('data-course-id');
        //不要回傳符合courseId和teacherId的組合
        selectedSubjects = selectedSubjects.filter(function (item) {
            return !(item.courseId === courseId && item.teacherId === teacherId);
        });
        //刪除在畫面中的組合 就是this的最近li element刪除
        $(this).closest('li').remove();     
    });

    //新增陣列
    $(document).on('click', '.selectTeacher', function (e) {
        e.preventDefault();

        var teacherId = $(this).attr('data-teacher-id');
        var teacherName = $(this).closest('tr').find('td:first').text();
        var courseId = $(this).attr('data-course-id');
        var courseName = $('#subjectSelect option:selected').text();

        // 檢查是否已經存在相同的科目和老師組合
        var exists = selectedSubjects.some(function (item) {
            return item.courseId === courseId && item.teacherId === teacherId;
        });

        // 如果已存在相同的組合，則不執行後續操作
        if (exists) {
            alert("該科目和老師組合已存在！");
            return;
        }

        //新增新的科目-老師組合 到陣列
        selectedSubjects.push({ courseId: courseId, teacherId: teacherId });

        //渲染畫面
        //step1 設置好button
        var button = $('<button>').text('刪除').addClass('btn btn-warning');
        //step2 先設置li元素並且加入屬性讓之後能取得老師id和課程id，1並且apend顯示的span 顯示科目名稱和老師名稱
        var listItem = $('<li>').addClass('list-group-item d-flex justify-content-between align-items-center')
            .attr({
                'data-teacher-id': teacherId,
                'data-course-id': courseId
            })
            .append($('<span>').addClass('fs-6 fw-bold text-muted w-35').text(courseName))
            .append($('<span>').addClass('fs-6 w-35').text(teacherName))
            .append(button);

        //新增到list裡
        $('#subjectTeacherList').append(listItem);
    });

    
});
