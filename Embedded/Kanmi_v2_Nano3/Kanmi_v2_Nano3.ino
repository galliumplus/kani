/*
 * Implémentation de Kanmi pour G+KR1 (Arduino NANO 3.0 + MFRC522) 
 */

#include <SPI.h> // Bibliothèque SPI standard
#include <MFRC522.h> // <MFRC522@1.4.11> by GithubCommunity

#define RST_PIN  3
#define SS_PIN  10
#define LED_PIN 19 

MFRC522 mfrc522 = MFRC522(SS_PIN, RST_PIN);

const uint8_t kanmiVersion = 0x02;  // version du protocole Kanmi

void printByte(uint8_t value);
void printBytes(uint8_t* value, uint16_t start, uint16_t end);

void ledBlink(uint16_t duration);

void setup() {
  // Initialisation de la communication avec le PC
  Serial.begin(115200);

  // Initialisation du bus SPI
  SPI.begin();

  // Initialisation du MFRC522
  mfrc522.PCD_Init();
  uint8_t mfrc522Version = mfrc522.PCD_ReadRegister(mfrc522.VersionReg);

  pinMode(LED_PIN, OUTPUT);
  ledBlink(200);
  delay(200);
  ledBlink(200);

  Serial.print(F("KANMI:READER_VERSION "));
  printByte(kanmiVersion);
  Serial.print(' ');
  printByte(mfrc522Version);
  Serial.println();

  delay(200);
  ledBlink(200);
}

void loop() {
  if (!mfrc522.PICC_IsNewCardPresent()) {
    // Vérification deux fois par seconde
    delay(500);
  } else if (!mfrc522.PICC_ReadCardSerial()) {
    ledBlink(200);
    delay(500);
  }else {
    Serial.print(F("KANMI:ENGAGED_WITH_PICC "));
    if (mfrc522.uid.size >= 4) {
      printBytes(mfrc522.uid.uidByte, 0, 4);
    }
    if (mfrc522.uid.size >= 7) {
      Serial.print('-');
      printBytes(mfrc522.uid.uidByte, 4, 7);
    }
    if (mfrc522.uid.size >= 10) {
      Serial.print('-');
      printBytes(mfrc522.uid.uidByte, 7, 10);
    }
    Serial.println();

    mfrc522.PICC_HaltA();
    ledBlink(800);
  }
}


void printByte(uint8_t value) {
  if (value < 0x10) Serial.print('0');
  Serial.print(value, HEX);
}

void printBytes(uint8_t* value, uint16_t start, uint16_t end) {
  for (uint16_t i = start; i < end; i++) {
    printByte(value[i]);
  }
}


void ledBlink(uint16_t duration) {
  digitalWrite(LED_PIN, HIGH);
  delay(duration);
  digitalWrite(LED_PIN, LOW); 
}
