# HotelCreatedEventHandler with idempotency

This project is an **AWS Lambda function** that handles hotel creation events published via **SNS**.  
It ensures **idempotency** by storing processed event IDs in **DynamoDB** and indexes hotel data into **Elasticsearch**.

---

## 🏗️ Architecture Overview

The event flow works as follows:

```text
          +----------------+
          | Hotel Service  |
          | (publishes SNS)|
          +--------+-------+
                   |
                   v
          +----------------+
          |   SNS Topic    |
          +--------+-------+
                   |
                   v
          +-----------------------------+
          |  Lambda: HotelCreatedEvent  |
          |  - Receives SNS Event       |
          |  - Checks DynamoDB          |
          |  - Indexes in Elasticsearch|
          +-----------------------------+
                   |
         +---------+---------+
         |                   |
         v                   v
+----------------+     +-----------------+
| DynamoDB Table |     | Elasticsearch   |
| hotel-created- |     | Index: "event"  |
| event-ids      |     |                 |
+----------------+     +-----------------+
