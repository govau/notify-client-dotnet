# Contributing

Pull requests are welcome.

## Working on the client locally

You will need to ensure that you have .Net 4.6.2 Framework and .Net core 2.0 installed on your machine.
If you are not working on a windows OS, .Net Frameworks are not supported but you can use the Makefile to build and run tests, run `make` on your terminal to see the available options.

You can run tests in a docker container locally by using the local Dockerfile as
a base (it just adds `make` to the fsharp:netcore image). Start with:

```
docker build -t fsnetcore .
```

You can then run tests or anything else through docker as follows:

```
docker run -v $(pwd):/src -w /src --env-file .env -it fsnetcore [COMMAND]

docker run -v $(pwd):/src -w /src --env-file .env -it fsnetcore make build
docker run -v $(pwd):/src -w /src --env-file .env -it fsnetcore make build-test
docker run -v $(pwd):/src -w /src --env-file .env -it fsnetcore make build-package
```

## Tests

Before running tests please ensure that the environment variables are set up for the test project.

```
NOTIFY_API_URL "https://example.notify-api.url"
API_KEY "example_API_test_key"
FUNCTIONAL_TEST_NUMBER "valid mobile number"
FUNCTIONAL_TEST_EMAIL "valid email address"
EMAIL_TEMPLATE_ID "valid email_template_id"
SMS_TEMPLATE_ID "valid sms_template_id"
SMS_SENDER_ID "valid sms_sender_id - to test sending to a receiving number, so needs to be a real number"
API_SENDING_KEY "API_whitelist_key for sending an SMS to a receiving number"
INBOUND_SMS_QUERY_KEY "API_test_key to get received text messages"
```
