const int in1 = 8;
const int in2 = 7;
const int ena = 6;
const int in3 = 12;
const int in4 = 11;
const int enb = 10;
const int PIN_D0 = 2;
volatile byte pulses;
unsigned long timeOld;
float speedCar = 20; //pwm 0-255;
int rpm;
String vstr = "";
char myChar;
boolean stringComplete = false;  // whether the string is complete
boolean motor_start = false;
void setup() {
  pinMode(in1,OUTPUT);
  pinMode(in2,OUTPUT);
  pinMode(ena,OUTPUT);
  pinMode(in3,OUTPUT);
  pinMode(in4,OUTPUT);
  pinMode(enb,OUTPUT);
  Serial.begin(9600);
  pinMode(PIN_D0, INPUT);
  pulses = 0;
  timeOld = 0;
  rpm = 0;
  attachInterrupt(0, counter, FALLING); // ngắt ngoài do chân chọn là 2 nên int = 0, hàm đếm counter, chế độ ngắt falling

}

void loop() {
  if (stringComplete) {
    vstr = "";
    stringComplete = false;
  }
  readdata();
  sensor();
 
}
 void counter()
 {
      pulses++;    
 }
 void sensor()
 {
  if (millis() - timeOld >= 1000) // (hàm millis thế cho delay) điều kiện này có nghĩa là hàm millis trừ thời gian củ đã lưu lớn hơn hoặc bằng 1s thì sẽ chạy điều kiện này.
  {
    
   timeOld = millis();
   rpm = float((pulses/20)*60);
   Serial.println(rpm);
   pulses = 0;
   
  }
 }
void tien()
{
  //motor A
  digitalWrite(in1,HIGH);
  digitalWrite(in2,LOW);
  analogWrite(ena,speedCar);
  //motor B
  digitalWrite(in3,HIGH);
  digitalWrite(in4,LOW);
  analogWrite(enb,speedCar);
}
void lui()
{
  //motor A
  digitalWrite(in1,LOW);
  digitalWrite(in2,HIGH);
  analogWrite(ena,speedCar);
  //motor B
  digitalWrite(in3,LOW);
  digitalWrite(in4,HIGH);
  analogWrite(enb,speedCar);
}
void stopmotor()
{
  digitalWrite(in1,LOW);
  digitalWrite(in2,LOW);
  digitalWrite(in3,LOW);
  digitalWrite(in4,LOW);
}
void readdata()
{
  if (vstr.substring(0,2) == "st")
  {
    stopmotor();
    speedCar = vstr.substring(2,vstr.length()).toFloat();
    tien();
    }
  if (vstr.substring(0,2) == "sl")
  {
    stopmotor();
    speedCar = vstr.substring(2,vstr.length()).toFloat();
    lui();
    }
  if (vstr.substring(0,1) == "a"){tien();}
  if (vstr.substring(0,1) == "b"){lui();}
  if (vstr.substring(0,1) == "c"){speedCar = 20; tien();} // start
  if (vstr.substring(0,1) == "s"){stopmotor();} // stop
  if (vstr.substring(0,1) == "t"){stopmotor();speedCar = speedCar +15; if (speedCar >= 255) {speedCar = 255;} tien();} // tăng tốc độ chiều forward (tiến)
  if (vstr.substring(0,1) == "g"){stopmotor();speedCar = speedCar - 15; if (speedCar <= 20) {speedCar = 20;}  tien();} // giảm tốc độ chiều forward
  if (vstr.substring(0,1) == "i"){stopmotor();speedCar = speedCar +15; if (speedCar >= 255) {speedCar = 255;} lui();}// tăng tốc độ chiều backward (lùi)
  if (vstr.substring(0,1) == "d"){stopmotor();speedCar = speedCar - 15; if (speedCar <= 20) {speedCar = 20;}  lui();}// giảm tốc độ chiều backward
}
void serialEvent() {
  while (Serial.available()) {

    char inChar = (char)Serial.read();

    if (inChar != '\n') {
      vstr += inChar;
    }

    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}
