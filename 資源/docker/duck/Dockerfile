# Use an official lightweight Python image.
# https://hub.docker.com/_/python
FROM python:3.9-slim

# Set the working directory in the container
WORKDIR /app

# Install dependencies:
# - FastAPI: Web framework for building APIs with Python 3.7+ based on standard Python type hints.
# - Uvicorn: ASGI server for FastAPI, for production use.
# - python-dotenv: To load environment variables from a .env file.
# - openai: Client library for accessing OpenAI's APIs.
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the current directory contents into the container at /app
COPY . .

# Command to run the uvicorn server:
# Use --host to bind to all network interfaces (both inside and outside the container)
# Use --reload to automatically reload the code on change (useful for development; remove in production)
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8080"]
