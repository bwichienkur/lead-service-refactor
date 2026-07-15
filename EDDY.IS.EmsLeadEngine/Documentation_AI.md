You are acting as a Principal Software Architect, Senior .NET Engineer, Solution Architect, and Technical Writer.

Your objective is to completely reverse engineer and document this .NET application as if you were preparing another engineering team to maintain, extend, and eventually rewrite it.

Do not simply describe code. Understand WHY it exists, how it works, and how each component interacts with the rest of the system.

Work through the solution systematically until every project has been analyzed.

For every conclusion, reference the code that led you to that conclusion. If something is uncertain, explicitly state your confidence level rather than guessing.

# Documentation Goals

Produce enterprise-level documentation that could be handed to a new engineering team.

The documentation should be written in Markdown and organized into folders.

---

# 1 Executive Summary

Describe:

- What the application does
- The business problem it solves
- Primary users
- Major workflows
- Major integrations
- High-level architecture
- Technology stack
- Deployment model
- Strengths
- Weaknesses
- Technical debt

---

# 2 Solution Overview

Document:

- Every project
- Purpose of each project
- Project references
- Dependency graph
- Startup sequence
- Application lifecycle

Generate a visual dependency diagram.

---

# 3 Architecture

Document the architecture style.

Examples:

- Layered
- Clean Architecture
- Onion
- Vertical Slice
- MVC
- CQRS
- Microservices
- Event Driven

Explain why the application was built this way.

Identify architectural violations.

---

# 4 Project Documentation

For every project:

Purpose

Responsibilities

Dependencies

Important classes

Configuration

External services

NuGet packages

Potential improvements

---

# 5 Folder Documentation

For every folder:

Purpose

Design pattern

Important classes

Relationships

Potential cleanup opportunities

---

# 6 Class Documentation

For every important class:

Purpose

Responsibilities

Collaborators

Lifecycle

Dependencies

Patterns used

Thread safety

Potential issues

Methods summary

Complexity

---

# 7 Business Logic Documentation

Identify every major business process.

For each process explain:

Inputs

Outputs

Validation

Rules

Edge cases

Failure handling

Related classes

Related database tables

Sequence diagram

Examples

---

# 8 API Documentation

Document every endpoint.

Method

URL

Authentication

Request

Response

Validation

Error handling

Business logic

Dependencies

---

# 9 Database Documentation

Identify:

Every DbContext

Every Entity

Every Table

Relationships

Indexes

Foreign keys

Stored procedures

Views

Functions

Triggers

Migration history

Soft delete strategy

Audit fields

Generate an ER diagram.

If relationships are inferred rather than explicit, explain why.

---

# 10 Entity Documentation

For every entity:

Purpose

Relationships

Navigation properties

Business meaning

Validation

Lifecycle

---

# 11 Services

Document every service.

Responsibilities

Dependencies

Consumers

External APIs

Retry policies

Caching

Logging

Error handling

Potential improvements

---

# 12 Dependency Injection

Generate a dependency graph.

Document:

Singletons

Scoped

Transient

Factories

Hosted Services

Background Services

---

# 13 Configuration

Document:

appsettings

Environment variables

Secrets

Feature flags

Connection strings

Configuration binding

Options classes

---

# 14 Authentication

Document:

Authentication

Authorization

JWT

Cookies

Claims

Roles

Policies

Permission model

---

# 15 Background Processing

Document:

Hosted Services

Hangfire

Quartz

Azure Functions

Timers

Queues

Cron schedules

Retry logic

---

# 16 Integrations

For every external system document:

Purpose

Authentication

Endpoints

Retry logic

Failure handling

Data mapping

Rate limits

Configuration

Sequence diagrams

---

# 17 Logging

Document:

ILogger usage

Serilog

NLog

Telemetry

Application Insights

Structured logging

Correlation IDs

---

# 18 Exception Handling

Document:

Global handlers

Middleware

Retry logic

Compensation

Error responses

Failure flows

---

# 19 Security Review

Identify:

Secrets

Hardcoded credentials

Injection risks

Authorization gaps

Input validation

CSRF

XSS

SSRF

Open redirects

Privilege escalation

Dependency vulnerabilities

Rate limiting

PII handling

Provide recommendations.

---

# 20 Performance Review

Analyze:

Memory allocations

LINQ inefficiencies

Large object allocations

N+1 queries

Async correctness

Blocking calls

Database performance

Caching opportunities

Parallelization

Hot paths

---

# 21 Code Quality

Identify:

Code smells

God classes

Long methods

Duplicated logic

Dead code

Unused services

Circular dependencies

Naming issues

SOLID violations

DRY violations

Large interfaces

Overengineering

Underengineering

---

# 22 Design Patterns

Identify every design pattern used.

Examples:

Repository

Factory

Mediator

Decorator

Strategy

Observer

Specification

Builder

Adapter

Facade

Unit of Work

Explain how each is implemented.

---

# 23 Testing

Document:

Unit tests

Integration tests

Coverage

Missing tests

Mocking strategy

Recommendations

---

# 24 Deployment

Document:

Build process

CI/CD

Infrastructure

Docker

Kubernetes

Azure

IIS

Environment promotion

Configuration differences

---

# 25 Data Flow

Generate diagrams showing:

Request lifecycle

Database interactions

Service interactions

External API interactions

Authentication flow

Background processing

---

# 26 Sequence Diagrams

Generate Mermaid sequence diagrams for all major workflows.

---

# 27 Class Diagrams

Generate Mermaid class diagrams.

---

# 28 Flow Charts

Generate Mermaid flowcharts for important business processes.

---

# 29 Dependency Graphs

Generate Mermaid dependency graphs.

---

# 30 Refactoring Recommendations

Rank improvements by impact.

For each recommendation provide:

Problem

Risk

Estimated effort

Business impact

Technical impact

Suggested implementation

Priority

---

# 31 AI Knowledge Base

Create an "AI_CONTEXT.md" file that summarizes:

Architecture

Business rules

Naming conventions

Coding standards

Patterns

Important workflows

Database relationships

External systems

This document should allow another AI agent to immediately understand the application.

---

# 32 Documentation Structure

Generate the following folder structure:

/Documentation

    ExecutiveSummary.md

    Architecture.md

    BusinessProcesses.md

    APIs/

    Database/

    Entities/

    Services/

    Projects/

    Security/

    Performance/

    Deployment/

    Diagrams/

    Refactoring/

    AI_CONTEXT.md

---

Continue until every project has been analyzed.

Do not stop after one project.

Track progress.

Keep documentation internally consistent.

Whenever possible, generate Mermaid diagrams.

Your goal is to produce documentation that is sufficiently detailed for another engineering team—or another AI agent—to understand, maintain, extend, and modernize the application without requiring the original developers.
