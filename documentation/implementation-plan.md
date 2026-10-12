# Implementation Plan for Full Harvest API Coverage

**Status: complete.** Every operation documented in the Harvest API v2 reference is implemented (32 resource
interfaces, 149 public methods); see [README.md](README.md#coverage) for the coverage summary and what remains to be
verified against the live API.

## How coverage was reached (3.0)

1. **Core hardening**: a required `CancellationToken` on every method, `ListAsync` methods taking `ListOptions`,
   `HarvestApiException`, retries of throttled and transient failures, a configurable `User-Agent`, and a unit test
   harness that pins every request without touching the network.
2. **Resource groups**, each built to [docs/IMPLEMENTING.md](../docs/IMPLEMENTING.md) with full unit tests:
   - reports;
   - expenses, expense categories and client contacts;
   - estimates, estimate messages and estimate item categories;
   - invoices, invoice messages, invoice payments and invoice item categories;
   - the PTO (time off) API;
   - roles, teammates, billable and cost rates, project user assignments, the global task assignments list, time entry
     restart/stop/external reference deletion, company update, and the Harvest ID accounts endpoint.

## Remaining work

- **Live verification**: exercise the resources added in 3.0 against a real (non-production) Harvest account and correct
  any field that Harvest sends differently from its documentation.
- **Harvest changes**: when Harvest documents a new endpoint or field, add it following
  [docs/IMPLEMENTING.md](../docs/IMPLEMENTING.md).
