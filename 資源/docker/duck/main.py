from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import time
import os
from dotenv import load_dotenv
from openai import OpenAI
import uvicorn
import json

# 初始化 FastAPI
app = FastAPI()

# 添加 CORS 中間件
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # 允許所有來源的請求
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# 載入環境變數
load_dotenv('.env')

# 獲取 API 金鑰
api_key = os.environ.get("api_key")
client = OpenAI(api_key=api_key)
assistant_id = "asst_QD0lFUg1yOixHEl4XE2igBdW"

# 定義用戶輸入的 Pydantic 模型
class UserInput(BaseModel):
    questions: int

# 提交用戶訊息的函數
def submit_message(assistant_id, thread, user_message):
    client.beta.threads.messages.create(
        thread_id=thread.id, role="user", content=user_message
    )
    return client.beta.threads.runs.create(
        thread_id=thread.id,
        assistant_id=assistant_id,
    )

# 獲取回應的函數
def get_response(thread):
    messages = client.beta.threads.messages.list(thread_id=thread.id, order="asc")
    message_json = json.loads(messages.data[1].content[0].text.value)
    return message_json

# 等待執行的函數
def wait_on_run(run, thread):
    while run.status == "queued" or run.status == "in_progress":
        run = client.beta.threads.runs.retrieve(
            thread_id=thread.id,
            run_id=run.id,
        )
        time.sleep(0.5)
    return run

# 定義 POST 路由
@app.post("/generate_questions/")
async def generate_questions(user_input: UserInput):
    # 建立對話和執行
    thread = client.beta.threads.create()
    user_message = f"請提供我{user_input.questions}題"
    run = submit_message(assistant_id, thread, user_message)

    # 等待執行結束
    run = wait_on_run(run, thread)

    # 獲取回應並返回
    content = get_response(thread)
    return content

# 允許從外部訪問的 FastAPI 應用程式
if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8080)
