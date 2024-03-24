
let connection;
let groupName;
let localStream;
let peer;
var start = 0;
var record = false;
var sound = false;


$(function () {

    // <summary>
    // 在畫面初始時就先直接建立hub中的班級科目群組
    // STEP 1:初始化 SignalR connection，連去StreamHub(管理直播的HUB)
    // => return: "SignalR connection established." 、 "SignalR connection error: "
    // STEP 1.2:初始化 SignalR connection 成功，創建班級科目id的群組，invoke("createGroup,"班級科目id)
    // STEP 1.3:綁定的"groupJoinState"會知道是否有創建群組成功
    // </summary>

    //1.初始配置 SignalR connection，連去StreamHub
    connection = new signalR.HubConnectionBuilder().withUrl("/StreamHub").build();

    //1.開始SignalR連線
    connection.start().then(function () {
        console.log("SignalR connection established.");
        //2.創建班級科目id的群組
        connection.invoke("createGroup", `${classCourseId}`);
    }).catch(function (err) {
        console.error("SignalR connection error: ", err);
    });

    //3.綁定的"groupJoinState"
    connection.on("groupJoinState", function (data) {
        var obj = JSON.parse(data);
        if (obj.groupName != "") {
            groupName = obj.groupName;
        } else {
            console.log(groupName);
        }
        alert(obj.message);
    });




    connection.on("ReceiveStudentConnectId", function (studentPeerId) {
        if (localStream) {
            console.log("Local stream is available. Creating and sending offer to student.");
            console.log(studentPeerId);
            const call = peer.call(studentPeerId, localStream);

            call.on('stream', function (remoteStream) {

            });

            call.on('error', function (err) {
                console.error("Error during the call: ", err);
            });
        } else {
            console.log("Local stream is not available. Cannot create offer without local stream.");
        }
    });

});


// <summary>
// 使用者點擊相關的button事件:錄影(toggleRecording)、錄音、重整三個按鈕選擇
// STEP 1:第一次直播toggleRecording()，start==0、record & sound 為false
// STEP 2:圖示變化
// STEP 3:設置peerId、並透過hub通知學生回傳自己的peerId給這裡
// STEP 4:start++，表示在這個頁面上就不是第一次直播了
// STEP 5:綁定ReceiveStudentConnectId，收到學生的id才能設置peer
// </summary>
///<param name="start">是不是在這個畫面上第一次開啟直播，如果為0代表是第一次。</param>
///<param name="record">當前是否影像端可以傳送，true為可傳送、false表示不傳送。</param>
///<param name="sound">當前是否音訊端可以傳送，true為可傳送、false表示不傳送</param>

function toggleRecording() {
    try {
        record = !record;
        sound = !sound;
        var localVideo = document.getElementById('localVideo');
        var waitingGif = document.getElementById('waitingGif');
        var recordIcon = document.getElementById('recordIcon');

        //檢查是否是第一次開啟直播，如果是要初始化數據流
        if (start === 0) {
            navigator.mediaDevices.getDisplayMedia({ video: true })
                .then(videoStream => {
                    navigator.mediaDevices.getUserMedia({ audio: true })
                        .then(audioStream => {
                            //設置本地流
                            localStream = new MediaStream([...videoStream.getVideoTracks(), ...audioStream.getAudioTracks()]);
                            document.getElementById('localVideo').srcObject = localStream;
                            //設置老師端的圖示變化，因為是第一次開啟直播所以直接寫死，video可看；wainting消失
                            localVideo.style.display = 'block';
                            waitingGif.style.display = 'none';
                            recordIcon.classList.replace("bi-record-circle", "bi-stop");
                            //傳送給後端告知已經開啟直播並且設置好peerId
                            setUpPeer();

                            ////讓學生端回傳peerId
                            //connection.invoke("informStudent", groupName);

                            //之後就不是第一次開啟
                            start++;
                        })
                        .catch(err => {
                            console.error("Error getting audio stream: ", err);
                        });
                })
                .catch(err => {
                    console.error("Error getting video stream: ", err);
                });
        } else {

            // 當不是第一次按這按紐，就是暫停或重新繼續直播
            if (record) {
                // 繼續直播，讓本來的流每個流都能被獲取
                localStream.getVideoTracks().forEach(track => {
                    track.enabled = true;
                });
                localStream.getAudioTracks().forEach(track => {
                    track.enabled = sound;
                });
                recordIcon.classList.replace("bi-stop", "bi-record-circle");
                localVideo.style.display = 'block';
                waitingGif.style.display = 'none';
            } else {
                // 當暫停直播後
                localStream.getTracks().forEach(track => {
                    track.enabled = false;
                });

                recordIcon.classList.replace("bi-record-circle", "bi-stop");
                localVideo.style.display = 'none';
                waitingGif.style.display = 'block';
            }
        }
    } catch (error) {
        console.error("An error occurred in toggleRecording: ", error);
    }
}


function reloadPage() {
    window.location.reload();
}



function togglemute() {
    //音訊連先關閉
    sound = !sound;
    localStream.getAudioTracks().forEach(track => {
        track.enabled = sound;
    });

    //將icon更換
    var micIcon = document.getElementById('soundIcon');
    if (sound) {
        micIcon.classList.replace("bi-mic-mute-fill", "bi-mic-fill");
    } else {
        micIcon.classList.replace("bi-mic-fill", "bi-mic-mute-fill");
    }
}

// 設置iceServers、使用peer.js
function setUpPeer() {
    peer = new Peer({
        config: {
            iceServers: [
                { urls: 'stun:stun.l.google.com:19302' },
                {
                    urls: "turns:global.relay.metered.ca:443?transport=tcp",
                    username: "73bbff3c8731ebccf9f03eae",
                    credential: "4Lc/Nvbuux92bsEG",
                },
            ]
        }
    });

    // On peer open
    peer.on('open', function (id) {
        console.log("Peer ID is: " + id);
    });

    // On peer error
    peer.on('error', function (err) {
        console.error("Peer.js error: ", err);
    });
}
