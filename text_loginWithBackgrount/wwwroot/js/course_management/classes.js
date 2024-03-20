

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
                        url: "/course_management/T課程班級/IndexJson",
                        dataSrc: function (json) { return json; },
                        //錯誤處理
                        error: function (xhr, textStatus, errorThrown) {
                            console.error("Error loading data:", errorThrown);
                            alert("Failed to load data. Please try again later.");
                        },
                    },
                    columns: [
                        { "data": "班級名稱", "width": "20%" },
                        {
                            data: '入學日期', width: '20%', render: function (data, type, row) {
                                //當渲染數據的模式是展示或搜尋時，回傳FORMATE 的日期
                                if (type === 'display' || type === 'filter') {
                                    var date = new Date(data);
                                    var formattedDate = date.getFullYear() + '-' + (date.getMonth() + 1).toString().padStart(2, '0') + '-' + date.getDate().toString().padStart(2, '0');
                                    return formattedDate;
                                }
                                //其他模式就回傳原本的數據
                                return data;
                            }
                        },
                        {
                            data: '結訓日期', width: '20%', render: function (data, type, row) {
                                //當渲染數據的模式是展示或搜尋時，回傳FORMATE 的日期
                                if (type === 'display' || type === 'filter') {
                                    var date = new Date(data);
                                    var formattedDate = date.getFullYear() + '-' + (date.getMonth() + 1).toString().padStart(2, '0') + '-' + date.getDate().toString().padStart(2, '0');
                                    return formattedDate;
                                }
                                //其他模式就回傳原本的數據
                                return data;
                            }
                        },
                        { "data": "班級導師姓名", "width": "20%" },
                        {
                            "data": "班級id",
                            "render": function (data, type, row) {
                                return '<p><a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_edit(' + row.班級id + ')"><i class="bx bx-edit-alt"></i></a></p>'
                            },
                            "orderable": false
                            , "width": "10%"

                        },
                        {
                            "data": "班級id",
                            "render": function (data, type, row) {
                                return '<p><a class="btn btn-warning" data-bs-toggle="modal" data-bs-target="#exampleModal" onclick = "popup_delete(' + row.班級id + ')"> <i class="bx bx-trash""></i> </a></p>'
                            },
                            "orderable": false
                            , "width": "10%"
                        },
                        {
                            "data": "班級id",
                            "render": function (data, type, row) {
                                return '<p><a class="btn btn-secondary"  onclick = "popup_detail(' + row.班級id + ')"><i class="bx bx-menu" ></i></a></p>'
                            },
                            "orderable": false
                            , "width": "10%"
                        },
                       
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

        //edit的方法 班級id
        function popup_edit(id) {

            //彈跳視窗的title更換
            exampleModalLabel.innerText = "Edit";

            $.ajax({
                type: 'GET',
                url: '/course_management/T課程班級/Edit/' + id,
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

        //delete的方法 班級id
        function popup_delete(id) {

            //彈跳視窗的title更換
            exampleModalLabel.innerText = "Delete";

            $.ajax({
                type: 'GET',
                url: '/course_management/T課程班級/Delete/' + id,
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
        function popup_detail(id) {
            window.location.href = '/course_management/T課程班級/Details/' + id;
        }
      
