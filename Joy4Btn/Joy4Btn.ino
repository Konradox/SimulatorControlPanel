#define BUTTON_1 30
#define BUTTON_2 31
#define BUTTON_3 32
#define BUTTON_4 33
#define ENCODER_CW 34
#define ENCODER_ACW 35
#define ENCODER_BTN 36
#define CONTROL_LED 13

bool prevState[10];
bool state;

void setup() {
  // put your setup code here, to run once:
   
  pinMode(CONTROL_LED, OUTPUT);
  pinMode(BUTTON_1, INPUT_PULLUP);
  pinMode(BUTTON_2, INPUT_PULLUP);
  pinMode(BUTTON_3, INPUT_PULLUP);
  pinMode(BUTTON_4, INPUT_PULLUP);
  pinMode(ENCODER_CW, INPUT_PULLUP);
  pinMode(ENCODER_ACW, INPUT_PULLUP);
  pinMode(ENCODER_BTN, INPUT_PULLUP);
  Serial.begin(9600);
}

void button(uint8_t pin)
{
  state = digitalRead(pin);
  if (state != prevState[pin - 30])
  {
    if (state == LOW)
      Serial.println(String(pin) + "1");
    else
      Serial.println(String(pin) + "0");
    digitalWrite(13,!state);
    prevState[pin - 30] = state;
  }
}

bool cwLast[3] = {LOW, LOW, LOW};
bool cwState = LOW;

void encoder(uint8_t clk, uint8_t dt, uint8_t id)
{
  cwState = digitalRead(clk);
  if ((cwLast[id] == LOW) && (cwState == HIGH)) 
  {
    if (digitalRead(dt) == LOW) 
    {
      Serial.println(String(id) + "+");
    } else 
    {
      Serial.println(String(id) + "-");
    }
  } 
  cwLast[id] = cwState;
}

void loop() {
  // put your main code here, to run repeatedly:
  button(BUTTON_1);
  button(BUTTON_2);
  button(BUTTON_3);
  button(BUTTON_4);
  button(ENCODER_BTN);
  encoder(ENCODER_CW, ENCODER_ACW, 0);
  encoder(38, 39, 1);
  encoder(40, 41, 2);
}
