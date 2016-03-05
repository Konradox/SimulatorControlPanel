#define BUTTON_1 30
#define BUTTON_2 31
#define BUTTON_3 32
#define BUTTON_4 33
#define CONTROL_LED 13

int prevState[4];
int state;

void setup() {
  // put your setup code here, to run once:
  pinMode(CONTROL_LED, OUTPUT);
  pinMode(BUTTON_1, INPUT_PULLUP);
  pinMode(BUTTON_2, INPUT_PULLUP);
  pinMode(BUTTON_3, INPUT_PULLUP);
  pinMode(BUTTON_4, INPUT_PULLUP);
  Serial.begin(9600);
}

void button(int pin)
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

void loop() {
  // put your main code here, to run repeatedly:
  button(BUTTON_1);
  button(BUTTON_2);
  button(BUTTON_3);
  button(BUTTON_4);
}
