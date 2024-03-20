
//彈跳視窗的title
let exampleModalLabel = document.getElementById("exampleModalLabel");




$(document).ready(
    function () {
        // <summary>
        // 在畫面初始時就先直接Datatable初始化
        // </summary>

        $("table").dataTable({
            ajax: {
                type: "GET",
                url: "/course_management/T課程科目/IndexJson",
                dataSrc: function (json) { return json; },
                //錯誤處理
                error: function (xhr, textStatus, errorThrown) {
                    console.error("Error loading data:", errorThrown);
                    alert("Failed to load data. Please try again later.");
                },
            },
            columns: [
                { "data": "科目名稱", "width": "30%" },
                { "data": "科目類別名稱", "width": "30%" },
                {
                    "data": "科目id",
                    "render": function (data, type, row) {
                        return '<p><a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_edit(' + row.科目id + ')"><i class="bx bx-edit-alt"></i></a></p>'
                    },
                    "orderable": false
                    , "width": "10%"

                },
                {
                    "data": "科目id",
                    "render": function (data, type, row) {
                        return '<p><a class="btn btn-warning" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_delete(' + row.科目id + ')"> <i class="bx bx-trash""></i> </a></p>'
                    },
                    "orderable": false
                    , "width": "10%"
                },
                {
                    "data": "科目id",
                    "render": function (data, type, row) {
                        return '<p><a class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_detail(' + row.科目id + ')"><i class="bx bx-menu" ></i></a></p>'
                    },
                    "orderable": false
                    , "width": "10%"
                },
                {
                    "data": "科目id",
                    "render": function (data, type, row) {
                        return '<p><a class="btn btn-success" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick ="popup_addTeacher(' + row.科目id + ', \'' + row.科目名稱 + '\')"><i class="bx bxs-plus-square" ></i></a></p>'
                    },
                    "orderable": false
                    , "width": "10%"
                }



            ],


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

        //create的方法 科目id
        function popup_create() {

            //彈跳視窗的title更換
                exampleModalLabel.innerText = "Create";

                $.ajax({
                    type: 'GET',
                    url: '/course_management/T課程科目/create',
                    success: function (result) {
                        console.log('Ajax request create success');
                        //彈跳視窗的內容更換
                        $("#switch_context").html("");
                        $("#switch_context").html(result);

                    },
                    error: function (error) {

                        console.error('Ajax request error', error);
                    }
                });
            }

        //edit的方法 科目id
        function popup_edit(id) {

                //彈跳視窗的title更換
                exampleModalLabel.innerText = "Edit";

                $.ajax({
                    type: 'GET',
                    url: '/course_management/T課程科目/Edit/' + id,
                    success: function (result) {

                        console.log('Ajax edit request success');
                        $("#switch_context").html("");
                        $("#switch_context").html(result);

                    },
                    error: function (error) {

                        console.error('Ajax request error', error);
                    }
                });

         }
       
        //delete的方法 科目id
        function popup_delete(id) {

            //彈跳視窗的title更換
            exampleModalLabel.innerText = "Delete";
            $.ajax({
                type: 'GET',
                url: '/course_management/T課程科目/Delete/' + id,
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

        //detail的方法 科目id
        function popup_detail(id) {

            //彈跳視窗的title更換
            exampleModalLabel.innerText = "Detail";
            $.ajax({
                type: 'GET',
                url: '/course_management/T課程科目/Details/' + id,
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

        //add teacher的方法 科目id 科目名稱
        function popup_addTeacher(id,name) {
            $.ajax({
                type: 'GET',
                url: '/course_management/T課程科目/CourseTeacher',
                success: async function (result) {
                    
                    console.log('Ajax CourseTeacher request success');
                    //彈跳視窗的title更換
                    exampleModalLabel.innerText = "add teacher to course";
                    //彈跳視窗的內容更換
                    $("#switch_context").html("");
                    $("#switch_context").html(result);
                    //顯示科目的名稱 並且將值設置成科目id，在視窗中固定
                    $("#chose_coursename").text(name);
                    $("#chose_courseid").val(id);

                    loadTeachersJson();
                   loadCourseTeachersJson(id);

                },
                error: function (error) {

                    console.error('Ajax request error', error);
                }
            });

}

        //add teacher的方法。跟後端拿取目前所有老師的資料，並透過renderTeachers 以datatable呈現
         async function loadTeachersJson() {
            try {
                
                const json = await $.ajax(
                    {
                    type: 'GET',
                        url: '/course_management/T課程科目/allTeacherJson',
                    }
                );
                await renderTeachers(json);
            } catch (error) {
                console.error('Error loading teachers', error);
            }

}

        //add teacher的方法。render同時綁定click，讓使用者選擇老師時會自動帶到表單裡面去利用表單跟後端互動
        async function renderTeachers(json){
            const table = $('#table_teacher').DataTable({
                data: json,
                columns: [
                   
                    { "data": '姓名' },
                    { "data": '手機' },
                    { "data": '信箱' },
                    {
                        "data": "老師id",
                        "render": function (data, type, row) {
                            return '<p><a class="btn btn-success" > <i class="bx bxs-plus-square"></i> </a></p>'
                        },
                        "orderable": false
                        , "width": "10%"
                    }
                ], 
                rowCallback: function (row, data) {

                    $(row).on('click', function () {
                        $('#chose_techername').text(data['姓名']);
                        $('#chose_techer').val(data['老師id']);
                    });
                }
            });
}

        //add teacher的方法。跟後端拿取目前所有科目可以授課老師老師的資料，並透過renderCourseTeachers 以datatable呈現
        async function loadCourseTeachersJson(id) {
            try {

                const json = await $.ajax(
                    {
                        type: 'GET',
                        url: '/course_management/T課程科目/allcourseTeacherJson/'+id,
                    }
                );
                await renderCourseTeachers(json);
            } catch (error) {
                console.error('Error loading teachers', error);
            }

}
        //add teacher的方法。 delete_courseTeacher在這裡被datatable引用。透過onclick綁定去跟後端互動刪除已經存在的授課老師
        async function renderCourseTeachers(json) {
            const table = $('#table_courseteacher').DataTable({
                data: json,
                columns: [
                   
                    { "data": '姓名' },
                    { "data": '手機' },
                    { "data": '信箱' },
                    {
                        "data": "老師id",
                        "render": function (data, type, row) {
                            var courseid = $("#chose_courseid").val(); 
                            return '<p><a class="btn btn-warning" onclick="delete_courseTeacher(' + row.老師id + ', ' + courseid + ')"> <i class="bx bx-trash"></i> </a></p>';

                        },
                        "orderable": false
                        , "width": "10%"
                    }
                ]
            });
        }
           
        
        //add teacher的方法。跟後端互動刪除已經存在的授課老師
        function delete_courseTeacher(teacherid, courseid) {
            $.ajax({
                type: 'POST',
                url: '/course_management/T課程科目/DeleteCourseTeacherConfirmed',
                data: { teacherid: teacherid, courseid: courseid },
                success: function (result) {
                    window.location.href = '/course_management/T課程科目/Index';

                },
                error: function (error) {
                    console.error('Ajax request error', error);

                }
            });
        }
        