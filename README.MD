## Some ideas that have not been implemented:

- Add more sophisticated retry strategy (for external failures) (maybe that particular phone number does not work for that particular provider) - if one provider fails, try another instead of trying to use the same provider for that channel. For example config for the sms channel: ["twilio", "vonage", "amazon_sns", "twilio", "twilio", "twilio", "twilio"]

- Add circuit breaker pattern for the external providers. Consider Polly, Microsoft.Extensions.Resilience

- Add external messaging broker

- Use time buckets instead of exact times

- Add endpoints for batch notifications
