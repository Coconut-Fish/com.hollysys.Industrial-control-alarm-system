#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>

void sendPostRequest(String source, String type, int level, String additionalInfo);

// put function declarations here:
int ledValue = LOW;

void setup() {
  pinMode(3,INPUT_PULLDOWN);
  pinMode(10,OUTPUT);
  Serial.begin(115200);
  //连接wifi
  WiFi.begin("YOUR-WIFI-UUID", "YOUR-WIFI-PASSWORD");
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the WiFi network");
  Serial.println(WiFi.localIP());
}
 
void loop() {
  if(digitalRead(3) == HIGH)
  {
    delay(10);
    sendPostRequest("esp32","button",2," ");
    delay(10);
    while (digitalRead(3) == HIGH) ;
  }
}

void sendPostRequest(String source, String type, int level, String additionalInfo) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    http.begin("http://***.***.***.***/api/Alarm/AddAlarm"); // 替换为你的URL
    http.addHeader("Content-Type", "application/json");

    // 构建JSON数据
    String jsonData = "{\"source\":\"" + source + "\",\"type\":\"" + type + "\",\"level\":" + String(level) + ",\"additionalInfo\":\"" + additionalInfo + "\"}";

    // 发送HTTP POST请求
    int httpResponseCode = http.POST(jsonData);

    // 打印响应
    if (httpResponseCode > 0) {
      String response = http.getString();
      Serial.println(httpResponseCode);
      Serial.println(response);
    } else {
      Serial.print("Error on sending POST: ");
      Serial.println(httpResponseCode);
    }

    // 结束HTTP连接
    http.end();
  } else {
    Serial.println("WiFi Disconnected");
  }
}