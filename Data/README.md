# Portfolio Data

This directory holds the structured content the API, MCP tools, and the resume PDF generator all read from.

- `profile.json`, `skills.json`, `projects.json`, `experience.json`, `education.json` — filled in from Mohamed Hanifa's resume. Read directly by the MCP tools (`Portfolio.Api/MCP/Tools/`), the `GET /api/profile|skills|projects|experience|education` endpoints, and `GET /api/resume/pdf` (which composes an ATS-friendly PDF from all of them on the fly - see `Portfolio.Api/Resume/`). This is what the AI assistant's answers are actually grounded in (the project is Grok-only, no RAG/embeddings pipeline).
- There is no `resume.pdf` file here on purpose - the resume is generated on demand from the JSON above (`GET /api/resume/pdf`), not served as a static file, so it's always in sync with whatever's in these files.

Two data points worth double-checking against the source:

- Synergein Technology LLC is listed as **Oct 2025 – Sep 2026** — an end date in the future relative to today. Kept as printed rather than assumed to mean "Present".
- No LinkedIn URL was on the resume, so `profile.json`'s `linkedin` field is left empty.
