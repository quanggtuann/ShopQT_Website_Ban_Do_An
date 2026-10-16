# VNPay sandbox setup

The checkout uses VNPay only after all required settings are present. The merchant secret is intentionally absent from tracked configuration. For local Development on this machine, the existing sandbox key is stored in .NET User Secrets for `ShopAPI` and in the Git-ignored `ShopAPI/appsettings.Development.local.json`. The API loads that local file after the normal configuration providers, including when Visual Studio starts the app from `bin/Debug/net8.0`. The local file is copied to the build output but excluded from publish output.

For another machine or deployment, provide `Vnpay:HashSecret` through that environment's secret store (or `Vnpay__HashSecret` as an environment variable). `Vnpay:TmnCode` and the sandbox endpoint are in `appsettings.json`. Do not copy the key into tracked configuration.

Set these HTTPS URLs for the environment where you test:

- `Vnpay__PaymentBackReturnUrl`: browser-accessible API URL ending in `/api/payments/vnpay/return` (local HTTPS works for browser testing).
- `Vnpay__FrontendResultUrl`: ShopView URL ending in `/Order/PaymentResult`.
- Register the public API URL ending in `/api/payments/vnpay/ipn` with VNPay as the IPN URL. This URL is configured with VNPay, not sent as a payment request parameter.

The browser return and ShopView result URLs for this localhost sandbox are provided in `appsettings.json`. Override both URLs for any environment using different hosts or ports. With the sandbox key present, selecting VNPay and opening the gateway works with these localhost URLs. `localhost` cannot receive VNPay's server-to-server IPN. For local Development using the VNPay sandbox host only, the API verifies the signed browser return and records the payment result so the order can continue through admin processing. Other environments use the signed IPN to set `Order.IsPaid`; configure a public HTTPS IPN URL for deployment.

An unpaid VNPay order can be cancelled while a payment page is still open. A later signed successful result leaves the order cancelled and records the attempt as `RefundRequired` for manual review. A paid VNPay order cannot be cancelled through the normal order action until a refund workflow is completed.

For this solution, ShopAPI runs at `https://localhost:7130/` and ShopView runs at `https://localhost:7106/`. The browser return is `/api/payments/vnpay/return` on ShopAPI; after signature verification it redirects to `/Order/PaymentResult` on ShopView. A sample callback such as `http://localhost:5261/Checkout/PaymentCallbackVnpay` belongs to a different route and port and should not be copied into this project. The `PaymentCallBack` section from that sample is not used here.

Apply the new database migrations to the configured development database before running the updated API:

```powershell
dotnet ef database update --project ShopDAL/ShopDAL.csproj --startup-project ShopAPI/ShopAPI.csproj --context ApplicationDbContext
```

If EF tooling cannot read the user NuGet configuration in a restricted shell, build with `dotnet build ShopAPI/ShopAPI.csproj --no-restore` and run the EF command with `--no-build`.

The integration supports payment initiation, signed return verification, idempotent IPN processing, retry from order details, and marking cancelled or duplicate paid transactions as `RefundRequired` for manual review. Production use additionally needs a tested refund/reconciliation workflow and monitoring for `RefundRequired` attempts.
