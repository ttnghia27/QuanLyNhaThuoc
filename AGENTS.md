# AGENTS.md

## Project overview

This project uses the following stack:

- **Frontend:** Vue 3 + TypeScript
- **Backend:** C#
- **Styling:** Tailwind CSS
- **UI components:** shadcn-vue

When making changes, follow the conventions below and prefer consistency with the existing project structure and patterns.

## Vue 3 and TypeScript conventions

### Use Composition API with `<script setup>`

- Prefer Vue 3 Composition API and the `<script setup lang="ts">` syntax for Single File Components.
- Use `ref`, `computed`, `watch`, and lifecycle hooks from Vue where appropriate.
- Keep component logic readable and focused. Extract reusable logic into composables when it is shared or substantial.
- Avoid the Options API unless an existing part of the codebase requires it for consistency.

Example:

```vue
<script setup lang="ts">
import { computed, ref } from "vue";

interface UserCardProps {
    name: string;
    role?: string;
}

const props = defineProps<UserCardProps>();

const emit = defineEmits<{
    select: [userName: string];
}>();

const isExpanded = ref(false);

const displayName = computed(() => props.name.trim());

function selectUser() {
    emit("select", displayName.value);
}
</script>
```

### Type component props with interfaces

- Define component-specific props using a local `interface` in the component when the type is only used by that component.
- Use `defineProps<InterfaceName>()` rather than relying on untyped props or broad `any` types.
- For generic components, use Vue's generic SFC support where appropriate, for example:

```vue
<script setup lang="ts" generic="TItem">
interface Props<T> {
    items: T[];
    selected?: T;
}

const props = defineProps<Props<TItem>>();
</script>
```

- Keep generic types as specific as possible. Avoid `any`; use `unknown` when the type is genuinely not known and narrow it before use.

### Type emits using tuple syntax

Prefer Vue 3.3+ tuple-based `defineEmits` typing:

```ts
const emit = defineEmits<{
    update: [value: string];
    select: [id: number, source: "mouse" | "keyboard"];
    close: [];
}>();
```

Avoid the older call-signature style for new components unless project compatibility requires it.

### Shared and global types

- Put reusable application-wide types in a shared `type.ts` file near the source root (for example, `src/type.ts` or the existing equivalent in this repository).
- Shared API contracts belong there, including common API response, pagination, error, and request types.
- Keep types that are only relevant to one component or feature local to that component or feature.
- Do not duplicate shared interfaces across files. Import the canonical definition instead.
- Do not place every type in the global file: keep feature-specific types close to the feature that owns them.

Example shared API response type:

```ts
export interface ApiResponse<T> {
    data: T;
    message?: string;
    success: boolean;
}

export interface ApiError {
    message: string;
    code?: string;
}
```

Use the actual response contract of the C# backend; do not assume every endpoint returns the same shape if the backend defines different contracts.

## Asynchronous code and API calls

- Use `async`/`await` for asynchronous operations instead of promise `.then()` chains.
- Prefer the project's existing API client/service layer for HTTP requests. Do not introduce raw `fetch` calls when an API client is already configured.
- Keep HTTP request logic out of presentation components when it can reasonably live in an API service or composable.
- Handle loading, success, empty, and error states explicitly where relevant.
- Use `try`/`catch` for errors that need local handling, and ensure loading flags are reset in `finally`.
- Do not silently swallow errors. Surface a useful message or rethrow the error when appropriate.

Example:

```ts
async function loadUsers() {
    isLoading.value = true;
    errorMessage.value = "";

    try {
        users.value = await userService.getUsers();
    } catch (error: unknown) {
        errorMessage.value =
            error instanceof Error ? error.message : "An unexpected error occurred.";
    } finally {
        isLoading.value = false;
    }
}
```

## UI components and styling

### Use shadcn-vue components

- Before building a UI element from scratch, check whether an appropriate shadcn-vue component is available.
- Add required components using the shadcn-vue CLI:

```bash
pnpm dlx shadcn-vue@latest add [Components]
```

- Replace `[Components]` with the required component name or names, following the CLI's supported syntax.
- Reuse and compose the generated components rather than copying their implementation into feature components.
- Do not manually recreate common primitives such as dialogs, dropdowns, forms, tabs, or tooltips when a suitable shadcn-vue component exists.

### Tailwind CSS

- Use Tailwind utility classes for layout, spacing, responsive behavior, and common visual styling.
- Follow the project's existing design tokens and theme configuration.
- Prefer consistent spacing and responsive layouts over one-off arbitrary values.
- Avoid adding custom CSS when Tailwind utilities or existing component variants can express the design cleanly.
- If custom CSS is necessary, scope it to the component and keep it minimal.

## C# backend integration and API Documentation

- Treat the C# backend API contract as the source of truth for request and response shapes.
- Keep frontend TypeScript types aligned with the backend DTOs and API behavior.
- Do not guess endpoint paths, field names, nullability, or enum values. Inspect the existing backend implementation or API documentation first.
- Keep frontend API calls in a dedicated service/API layer where the project structure supports it.
- Handle backend validation errors and non-success responses in a predictable way.
- Read `API-DOCS.md` at the project root.
- Treat `API-DOCS.md` as the primary reference for API endpoints, request parameters, response schemas, and authentication.
- Do not invent API endpoints, HTTP methods, request fields,
  response fields, or status codes.
- Ensure frontend TypeScript interfaces match the documented API contracts.
- If the API documentation conflicts with the actual C# backend
  implementation, inspect the backend and report the discrepancy.
- Update the frontend API service layer consistently with the documentation.
- Always consult `API-DOCS.md` before writing API integration code.

## Current application architecture

### Frontend responsibilities

- `Frontend/src/App.vue` renders the active Vue Router route; it does not own feature data.
- `Frontend/src/router/index.ts` defines routes and the client-side authentication guard. The root route renders `Frontend/src/views/MainLayoutView.vue`.
- `MainLayoutView.vue` is the authenticated application shell: it owns navigation and feature data/loading state, calls the API client, and passes data/actions to screen views.
- `Frontend/src/views/` contains screen-level components. Prefer typed props and emits; keep HTTP calls out of views.
- `Frontend/src/components/` contains reusable UI and feature components.
- `Frontend/src/services/api.ts` is the frontend HTTP boundary. Add or update endpoint calls there rather than introducing component-level `fetch` calls.
- `Frontend/src/type.ts` contains shared API and feature types. Keep those shapes aligned with the backend DTOs and `API-DOCS.md`.

### Backend responsibilities

- `Backend/QuanLyNhaThuoc.API/Program.cs` is the composition root: configure the `DbContext`, dependency injection, authentication, filters, and middleware there.
- `Program.cs` applies a fallback authorization policy requiring an authenticated user. Keep new API endpoints protected by default; mark only intentional public endpoints (such as login) with `[AllowAnonymous]`, and document the contract in `API-DOCS.md`.
- `Controllers/` handles HTTP concerns only: bind route/query/body values, call the corresponding service, and translate results into documented HTTP responses. Do not put EF Core queries, transactions, or business workflows in controllers.
- `Services/` owns application/business operations and data access. Keep service methods asynchronous and scoped to the feature they serve.
- `DTOs/` defines request and response contracts; `Models/` and `Data/ApplicationDbContext.cs` define persistence entities and EF Core mappings.
- `Helpers/CodeGenerator.cs` uses the database scalar functions for generated identifiers. Reuse it rather than duplicating identifier-generation logic.
- `Services/StoredProcedureExecutor.cs` is the SQL Server boundary for documented stored procedures. Use parameterized procedure parameters; do not build SQL from request values.
- `BusinessRuleException` represents expected business failures and `BusinessRuleExceptionFilter` maps them to the API's `400` `{ "message": "..." }` response. Return `null` for a missing entity when the controller needs to return `404`. Avoid broad catches that turn unexpected infrastructure failures into successful or business-error responses.
- Use the stored procedures and triggers described in `SQL-DOCS.md` for the workflows they own. Do not repeat side effects that the database already performs, such as stock and total updates.

The normal request flow is:

`Vue route -> MainLayoutView.vue -> screen view -> Frontend/src/services/api.ts -> API controller -> feature service -> ApplicationDbContext or SQL Server routine`

## General implementation practices

- Inspect nearby files and follow established naming, folder, and architectural conventions before introducing new patterns.
- Prefer small, focused changes and reusable code over unnecessary abstractions.
- Avoid `any`, duplicated logic, dead code, and unrelated refactors.
- Do not add a new dependency when the existing stack already provides the required functionality.
- Preserve accessibility: use semantic elements, labels, keyboard-friendly interactions, and appropriate ARIA attributes when needed.
- Ensure TypeScript types are correct and run the relevant lint, type-check, test, and build commands available in the repository after making changes.
- If a requirement is ambiguous or conflicts with the existing codebase, inspect the implementation and choose the smallest consistent solution; explain any important trade-off.
